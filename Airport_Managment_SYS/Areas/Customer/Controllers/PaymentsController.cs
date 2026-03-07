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
        public async Task<IActionResult> pay()
        {
           var options= new SessionCreateOptions
           {
               PaymentMethodTypes = new List<string> {"card" },
               LineItems = new List<SessionLineItemOptions>(),
               Mode = "payment",
               SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Payments/success",
               CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Payments/cancel"

           };
            var user = await _userManager.GetUserAsync(User);
            if (user==null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            var reservation = await _ReservationRepository.GetOneAsync(
                    r => r.ApplicationUserId == user.Id,
                    includeFunc: q => q
                        .Include(r => r.Trip)
                            .ThenInclude(t => t.Airport_From)
                        .Include(r => r.Trip)
                            .ThenInclude(t => t.Airport_To)
                );

            if (reservation==null)
            {
                return RedirectToAction("Index", "Reservations", new { area = "Customer" });
            }
            var payment = new Payment
            {
                Total = reservation.Trip.Price,
                ApplicationUserId = user.Id
            };
            var trip = await _tripRepository.GetOneAsync(t=> t.Id == reservation.TripId);
              

            options.LineItems.Add(new SessionLineItemOptions
            {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(reservation.Trip.Price * 100),
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Trip from {trip.Airport_From.Name} to {trip.Airport_To.Name}"
                        }
                    },
                    Quantity = 1
            }

               
                
            );
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
        public IActionResult success()
        {
                       // Payment was successful — mark reservation paid and book seats
                       var userName = User.Identity?.Name;
                       var userTask = _userManager.GetUserAsync(User);
                       userTask.Wait();
                       var user = userTask.Result;
                       if (user == null) return View();

                       var reservation = _ReservationRepository.GetOneAsync(r => r.ApplicationUserId == user.Id).Result;
                       if (reservation == null) return View();

                       // mark reservation paid
                       reservation.IsPaid = true;

                       // load trip with seats
                       var trip = _tripRepository.GetOneAsync(t => t.Id == reservation.TripId, includeFunc: q => q
                            .Include(t => t.TripSeats)
                                .ThenInclude(ts => ts.Seat)).Result;

                       if (trip != null)
                       {
                           var availableSeats = trip.TripSeats.Where(ts => !ts.IsBooked && ts.Seat != null && ts.Seat.seatClassId == reservation.SeatClassId).Take(reservation.NumSeats).ToList();
                           foreach (var ts in availableSeats)
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
