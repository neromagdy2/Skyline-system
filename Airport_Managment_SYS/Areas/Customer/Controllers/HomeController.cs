using Airport_Managment_SYS.Areas.Customer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Linq;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<GovernerateState> _governerateStateRepository;
        private readonly IRepository<Trip> _tripRepository;
        public HomeController(IRepository<GovernerateState> governerateStateRepository, IRepository<Trip> tripRepository)
        {
        }

        {
            }
            searchTripsVM.States = (await _governerateStateRepository.GetAsync()).ToList();

            var trips = await _tripRepository.GetAsync
                (t => t.Airport_From.GovernerateStateId == searchTripsVM.DepartureCity &&
                t.Airport_To.GovernerateStateId == searchTripsVM.ArrivalCity);


            searchTripsVM.trips = trips;

            return View(searchTripsVM);
        }


    }
}
