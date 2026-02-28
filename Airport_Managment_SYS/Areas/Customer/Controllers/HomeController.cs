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

            searchTripsVM.trips=trips;
          
            return View(searchTripsVM);
        }
        
        
    }
}
