using Airport_Managment_SYS.Areas.Customer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<GovernerateState> _governerateStateRepository;
        private readonly IRepository<Trip> _tripRepository;
        public HomeController(IRepository<GovernerateState> governerateStateRepository, IRepository<Trip> tripRepository)
        {
            _governerateStateRepository = governerateStateRepository;
            _tripRepository = tripRepository;
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
            var trip = await _tripRepository.GetOneAsync(t => t.Id == id,
                includes: new System.Linq.Expressions.Expression<Func<Trip, object>>[]
                {
                    t => t.TripSeats,
                    t => t.Airport_From,
                    t => t.Airport_To
                });
            if (trip == null)
                return NotFound();

            var available = trip.TripSeats.Count(ts => !ts.IsBooked);
            return View(new DetailsTripVM { Trip = trip, AvailableSeats = available });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int tripId, int seatsToReserve)
        {
            var trip = await _tripRepository.GetOneAsync(t => t.Id == tripId,
                includes: new System.Linq.Expressions.Expression<Func<Trip, object>>[]
                {
                    t => t.TripSeats
                });
            if (trip == null)
                return NotFound();

            var unbooked = trip.TripSeats.Where(ts => !ts.IsBooked).Take(seatsToReserve).ToList();
            if (unbooked.Count < seatsToReserve)
            {
                // not enough seats, redirect back with message
                TempData["Error"] = "Not enough available seats.";
                return RedirectToAction(nameof(Details), new { id = tripId });
            }
            foreach (var ts in unbooked)
            {
                ts.IsBooked = true;
            }
            _tripRepository.Update(trip);
            await _tripRepository.CommitAsync();
            TempData["Success"] = "Seats reserved successfully.";
            return RedirectToAction(nameof(Details), new { id = tripId });
        }
    }
}
