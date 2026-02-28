
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
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
            var reservation = await _ReservationRepository.GetOneAsync(r => r.ApplicationUserId == user.Id);
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
                       return View();   
        }
        public IActionResult cancel()
        {
            return View();

        }
     }}
