using Airport_Managment_SYS.Areas.Customer.ViewModels;
using Airport_Managment_SYS.Repositories;
using Airport_Managment_SYS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<GovernerateState> _governerateStateRepository;
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<SeatClass> _seatClassesRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(IRepository<GovernerateState> governerateStateRepository, IRepository<Trip> tripRepository, IRepository<SeatClass> seatClassesRepository, IRepository<Reservation> reservationRepository, UserManager<ApplicationUser> userManager)
        {
            _governerateStateRepository = governerateStateRepository;
            _tripRepository = tripRepository;
            _seatClassesRepository = seatClassesRepository;
            _reservationRepository = reservationRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var governerateStates = await _governerateStateRepository.GetAsync();
            return View(new SearchTripsVM() { States = governerateStates });
        }

        [HttpGet]
        public async Task<IActionResult> SearchTrips(SearchTripsVM searchTripsVM)
        {
            if (!ModelState.IsValid)
            {
                searchTripsVM.States = await _governerateStateRepository.GetAsync();
                return View("Index", searchTripsVM);
            }
            searchTripsVM.States = (await _governerateStateRepository.GetAsync()).ToList();


            var trips = await _tripRepository.GetAsync(
                t => t.Airport_FromId == searchTripsVM.DepartureCity &&
                     t.Airport_ToId == searchTripsVM.ArrivalCity,
                includes: new System.Linq.Expressions.Expression<Func<Trip, object>>[]
                {
                    t => t.Airport_From,
                    t => t.Airport_To
                });


            searchTripsVM.trips = trips;

            return View(searchTripsVM);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == id,
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
                            .ThenInclude(s => s.SeatClass)
                    .Include(t => t.Airport_From)
                    .Include(t => t.Airport_To)
            );
            if (trip == null)
                return NotFound();

            var available = trip.TripSeats.Count(ts => !ts.IsBooked);
            var seatClasses = await _seatClassesRepository.GetAsync();
            var availableByClass = trip.TripSeats
                .Where(ts => !ts.IsBooked && ts.Seat != null)
                .GroupBy(ts => ts.Seat.seatClassId)
                .ToDictionary(g => g.Key, g => g.Count());

            return View(new DetailsTripVM
            {
                Trip = trip,
                AvailableSeats = available,
                SeatClasses = seatClasses,
                AvailableByClass = availableByClass
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int tripId, int seatsToReserve, int seatClassId, decimal totalPrice)
        {
            // totalPrice comes from client calculation and can be passed on to payment processing
            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == tripId,
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
            );
            if (trip == null)
                return NotFound();

            var availableForClass = trip.TripSeats.Count(ts => !ts.IsBooked && ts.Seat != null && ts.Seat.seatClassId == seatClassId);
            if (availableForClass < seatsToReserve)
            {
                TempData["Error"] = "Not enough available seats.";
                return RedirectToAction(nameof(Details), new { id = tripId });
            }

            // create a pending reservation and redirect to payment
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            var reservation = new Reservation
            {
                TripId = tripId,
                NumSeats = seatsToReserve,
                SeatClassId = seatClassId,
                TotalPrice = (decimal)totalPrice,
                ApplicationUserId = user.Id,
                IsPaid = false
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.CommitAsync();

            // redirect to payment flow
            return RedirectToAction("pay", "Payments", new { area = "Customer" });
        }

        // GET: /Customer/Home/Bookings
        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            var reservations = (await _reservationRepository.GetAsync(
                r => r.ApplicationUserId == user.Id,
                includeFunc: q => q
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_From)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_To),
                trackd: false
            )).ToList();

            return View(reservations);
        }
    }
}
