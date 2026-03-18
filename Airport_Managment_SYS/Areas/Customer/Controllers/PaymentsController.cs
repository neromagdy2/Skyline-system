using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Reservation> _ReservationRepository;
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<Seat> _seatRepository;

        public PaymentsController(UserManager<ApplicationUser> userManager, IRepository<Reservation> reservationRepository, IRepository<Trip> tripRepository, IRepository<Payment> paymentRepository, IRepository<Seat> seatRepository)
        {
            _userManager = userManager;
            _ReservationRepository = reservationRepository;
            _tripRepository = tripRepository;
            _paymentRepository = paymentRepository;
            _seatRepository = seatRepository;
        }
        public IActionResult Index()
        {
            var username = User.Identity.Name;
            var reservation = _ReservationRepository.GetOneAsync(r => r.ApplicationUser.UserName == username).Result;
            return View(reservation);
        }
        public async Task<IActionResult> pay(int id)
        {
           var options= new SessionCreateOptions
           {
               PaymentMethodTypes = new List<string> {"card" },
               LineItems = new List<SessionLineItemOptions>(),
               Mode = "payment",
               SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Payments/success?reservationId={id}",
               CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Payments/cancel"

           };
            var user = await _userManager.GetUserAsync(User);
            if (user==null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            var reservation = await _ReservationRepository.GetOneAsync(
                    r => r.Id == id && r.ApplicationUserId == user.Id,
                includeFunc: q => q
                    .Include(r => r.ReservationSeats)
                        .ThenInclude(rs => rs.Seat)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_From)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_To)
                );

            if (reservation==null)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            if (reservation.IsPaid)
            {
                TempData["info"] = "already paid";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            // Check if reserved seats are still available
            var trip = await _tripRepository.GetOneAsync(t => t.Id == reservation.TripId,
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat));

            if (trip != null)
            {
                var reservedSeatIds = reservation.ReservationSeats.Select(rs => rs.SeatId).ToList();
                var bookedSeats = trip.TripSeats
                    .Where(ts => reservedSeatIds.Contains(ts.SeatId) && ts.IsBooked)
                    .ToList();

                if (bookedSeats.Any())
                {
                    // Some seats are no longer available, redirect to booking page with message
                    TempData["Warning"] = "Oh no! Some of your selected seats have been taken by other travelers. Please choose different seats for your journey.";
                    return RedirectToAction("Details", "Home", new { area = "Customer", id = reservation.TripId });
                }
            }

            options.LineItems.Add(new SessionLineItemOptions
            {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(reservation.TotalPrice * 100),
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Trip from {trip.Airport_From.Name} to {trip.Airport_To.Name} - {reservation.NumSeats} seat(s)"
                        }
                    },
                    Quantity = 1
            });

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
        public IActionResult success(int reservationId)
        {
                       // Payment was successful — mark reservation paid and book specific seats
                       var user = _userManager.GetUserAsync(User).Result;
                       if (user == null) return View();

                       var reservation = _ReservationRepository.GetOneAsync(r => r.Id == reservationId, 
                           includeFunc: q => q
                               .Include(r => r.ReservationSeats)
                                   .ThenInclude(rs => rs.Seat)).Result;
                       
                       if (reservation == null) return View();

                       // mark reservation paid
                       reservation.IsPaid = true;

                       // load trip with seats
                       var trip = _tripRepository.GetOneAsync(t => t.Id == reservation.TripId, 
                           includeFunc: q => q
                               .Include(t => t.TripSeats)
                                   .ThenInclude(ts => ts.Seat)).Result;

                       if (trip != null)
                       {
                           // Book the specific seats from the reservation
                           var reservedSeatIds = reservation.ReservationSeats.Select(rs => rs.SeatId).ToList();
                           var tripSeatsToBook = trip.TripSeats
                               .Where(ts => reservedSeatIds.Contains(ts.SeatId))
                               .ToList();

                           foreach (var ts in tripSeatsToBook)
                           {
                               ts.IsBooked = true;
                           }
                           _tripRepository.Update(trip);
                       }

                       _ReservationRepository.Update(reservation);
                       _ReservationRepository.CommitAsync().Wait();

                       return View();   
        }
    }
}
