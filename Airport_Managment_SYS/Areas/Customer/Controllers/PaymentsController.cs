using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Airport_Managment_SYS.Utilities;

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
        private readonly ITicketService _ticketService;
        private readonly ISkyStreamEmailSender _emailSender;

        public PaymentsController(UserManager<ApplicationUser> userManager, IRepository<Reservation> reservationRepository, IRepository<Trip> tripRepository, IRepository<Payment> paymentRepository, IRepository<Seat> seatRepository, ITicketService ticketService, ISkyStreamEmailSender emailSender)
        {
            _userManager = userManager;
            _ReservationRepository = reservationRepository;
            _tripRepository = tripRepository;
            _paymentRepository = paymentRepository;
            _seatRepository = seatRepository;
            _ticketService = ticketService;
            _emailSender = emailSender;
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
        public async Task<IActionResult> success(int reservationId)
        {
            // Payment was successful — mark reservation paid and book specific seats
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return View();

            var reservation = await _ReservationRepository.GetOneAsync(r => r.Id == reservationId, 
                includeFunc: q => q
                    .Include(r => r.ReservationSeats)
                        .ThenInclude(rs => rs.Seat)
                    .Include(r => r.ApplicationUser)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_From)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_To));
            
            if (reservation == null) return View();

            // mark reservation paid
            reservation.IsPaid = true;

            // load trip with seats
            var trip = await _tripRepository.GetOneAsync(t => t.Id == reservation.TripId, 
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat));

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
            await _ReservationRepository.CommitAsync();

            // Generate PDF ticket and send email
            try
            {
                var ticketPdf = await _ticketService.GenerateTicketPdf(reservation);
                var ticketFileName = $"SKYSTREAM_Ticket_{reservation.Id:D6}.pdf";
                
                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #3b82f6 0%, #1e40af 100%); color: white; padding: 30px; border-radius: 10px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 28px;'>SKYSTREAM</h1>
                            <h2 style='margin: 10px 0; font-size: 18px; font-weight: normal;'>Flight Ticket Confirmation</h2>
                        </div>
                        
                        <div style='background: #f8f9fa; padding: 30px; border-radius: 10px; margin: 20px 0;'>
                            <h3 style='color: #333; margin-bottom: 15px;'>Reservation Details</h3>
                            <p><strong>Reservation ID:</strong> #{reservation.Id:D6}</p>
                            <p><strong>From:</strong> {reservation.Trip?.Airport_From?.Name}</p>
                            <p><strong>To:</strong> {reservation.Trip?.Airport_To?.Name}</p>
                            <p><strong>Departure:</strong> {reservation.Trip?.DateTime:MMM dd, yyyy HH:mm}</p>
                            <p><strong>Arrival:</strong> {reservation.Trip?.ArrivalDateTime:MMM dd, yyyy HH:mm}</p>
                            <p><strong>Total Amount:</strong> {reservation.TotalPrice:C} EGP</p>
                            <p><strong>Status:</strong> <span style='color: #10b981; font-weight: bold;'>PAID</span></p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px;'>
                            <p style='color: #666; font-size: 14px;'>Your flight ticket is attached to this email.</p>
                            <p style='color: #666; font-size: 14px;'>Thank you for choosing SKYSTREAM!</p>
                        </div>
                    </body>
                    </html>";

                await _emailSender.SendEmailWithAttachmentAsync(
                    user.Email,
                    $"SKYSTREAM Flight Ticket - Reservation #{reservation.Id:D6}",
                    emailBody,
                    ticketPdf,
                    ticketFileName
                );

                TempData["Success"] = "Payment successful! Your ticket has been sent to your email.";
            }
            catch (Exception ex)
            {
                TempData["Warning"] = "Payment successful, but there was an issue sending your ticket email. Please contact support.";
            }

            _ReservationRepository.Update(reservation);
            _ReservationRepository.CommitAsync().Wait();
                var payment = new Payment
                {
                    Total = (float)reservation.TotalPrice,
                    ApplicationUserId = user.Id
                };
                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.CommitAsync();

            return View(reservation);   
        }
        public IActionResult cancel()
        {
         
            TempData["Warning"] = "Payment was cancelled ";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
