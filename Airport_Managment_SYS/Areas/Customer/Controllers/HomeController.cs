using Airport_Managment_SYS.Areas.Customer.ViewModels;
using Airport_Managment_SYS.Repositories;
using Airport_Managment_SYS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<GovernerateState> _governerateStateRepository;
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<SeatClass> _seatClassesRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<ReservationSeat> _reservationSeatRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(IRepository<GovernerateState> governerateStateRepository, IRepository<Trip> tripRepository, IRepository<SeatClass> seatClassesRepository, IRepository<Reservation> reservationRepository, IRepository<ReservationSeat> reservationSeatRepository, UserManager<ApplicationUser> userManager)
        {
            _governerateStateRepository = governerateStateRepository;
            _tripRepository = tripRepository;
            _seatClassesRepository = seatClassesRepository;
            _reservationRepository = reservationRepository;
            _reservationSeatRepository = reservationSeatRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var seatClasses = await _seatClassesRepository.GetAsync();
            var governerateStates = await _governerateStateRepository.GetAsync();
            return View(new SearchTripsVM() { States = governerateStates ,seatClasses=seatClasses});
        }

        [HttpGet]
        public async Task<IActionResult> SearchTrips(SearchTripsVM searchTripsVM, int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                searchTripsVM.seatClasses = await _seatClassesRepository.GetAsync();
                searchTripsVM.States = await _governerateStateRepository.GetAsync();
                return View("Index", searchTripsVM);
            }
            searchTripsVM.States = (await _governerateStateRepository.GetAsync()).ToList();
           searchTripsVM.seatClasses = await _seatClassesRepository.GetAsync();

            var startDate = searchTripsVM.DepartureTime.Date;
            var endDate = startDate.AddDays(1);

            var trips = await _tripRepository.GetAsync(
                t => t.Airport_FromId == searchTripsVM.DepartureCity &&
                     t.Airport_ToId == searchTripsVM.ArrivalCity &&
                     t.DateTime >= startDate &&
                     t.DateTime < endDate,
                includeFunc: q => q
                    .Include(t => t.Airport_From)
                    .Include(t => t.Airport_To)
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
            );
            if (searchTripsVM.MaxPrice >0)
            {
                trips = trips.Where(t => t.Price < searchTripsVM.MaxPrice);

            }
            if (searchTripsVM.SeatClassIds != null && searchTripsVM.SeatClassIds.Any())
            {
                trips = trips.Where(t =>
                    t.TripSeats != null &&
                    t.TripSeats.Any(ts =>
                      
                        ts.Seat != null &&
                        searchTripsVM.SeatClassIds.Contains(ts.Seat.seatClassId)
                    ));
            }
            
            // Pagination
            int totalItems = trips.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            trips = trips.Skip((page - 1) * pageSize).Take(pageSize);
            
            searchTripsVM.trips = trips;
            searchTripsVM.CurrentPage = page;
            searchTripsVM.PageSize = pageSize;
            searchTripsVM.TotalItems = totalItems;
            searchTripsVM.TotalPages = totalPages;

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
        public async Task<IActionResult> Reserve(ReserveVM reserveVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid reservation data.";
                return RedirectToAction(nameof(Details), new { id = reserveVM.TripId });
            }

            // Get the trip with seats
            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == reserveVM.TripId,
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
            );
            
            if (trip == null)
            {
                TempData["Error"] = "Trip not found.";
                return RedirectToAction(nameof(Index));
            }

            // Validate that all requested seat IDs exist for this trip and are not booked
            var tripSeatIds = trip.TripSeats
                .Where(ts => ts.Seat != null && ts.Seat.seatClassId == reserveVM.SeatClassId)
                .Select(ts => ts.SeatId)
                .ToHashSet();

            // Check if all requested seat IDs are valid for this trip and class
            var invalidSeats = reserveVM.SeatIds.Where(seatId => !tripSeatIds.Contains(seatId)).ToList();
            if (invalidSeats.Any())
            {
                TempData["Error"] = "Some selected seats are not available for this trip or seat class.";
                return RedirectToAction(nameof(Details), new { id = reserveVM.TripId });
            }

            // Check if any of the requested seats are already booked
            var bookedSeats = trip.TripSeats
                .Where(ts => reserveVM.SeatIds.Contains(ts.SeatId) && ts.IsBooked)
                .Select(ts => ts.SeatId)
                .ToList();

            if (bookedSeats.Any())
            {
                TempData["Error"] = "Some selected seats are already booked. Please select different seats.";
                return RedirectToAction(nameof(Details), new { id = reserveVM.TripId });
            }

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            // Calculate total price if not provided
            if (reserveVM.TotalPrice <= 0)
            {
                reserveVM.TotalPrice = (decimal)trip.Price * reserveVM.SeatIds.Count;
            }

            // Create reservation
            var reservation = new Reservation
            {
                TripId = reserveVM.TripId,
                NumSeats = reserveVM.SeatIds.Count,
                SeatClassId = reserveVM.SeatClassId,
                TotalPrice = reserveVM.TotalPrice,
                ApplicationUserId = user.Id,
                IsPaid = false
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.CommitAsync();

            // Create reservation seat entries
            foreach (var seatId in reserveVM.SeatIds)
            {
                var reservationSeat = new ReservationSeat
                {
                    ReservationId = reservation.Id,
                    SeatId = seatId
                };
                await _reservationSeatRepository.AddAsync(reservationSeat);
            }
            await _reservationSeatRepository.CommitAsync();

            // Redirect to payment with reservation ID
            return RedirectToAction("pay", "Payments", new { area = "Customer", id = reservation.Id });
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
