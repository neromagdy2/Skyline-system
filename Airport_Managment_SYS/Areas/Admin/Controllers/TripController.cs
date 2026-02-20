using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    public class TripController : Controller
    {
        private Repository<Trip> _TripRepo;
        private Repository<Trip> _AirplaneRepo;
        private Repository<Trip> _AirportRepo;
        public TripController(Repository<Trip> TripRepo, Repository<Trip> airplaneRepo, Repository<Trip> airportRepo)
        {
            _TripRepo = TripRepo;
            _AirplaneRepo = airplaneRepo;
            _AirportRepo = airportRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Trips = await _TripRepo.GetAsync();

            return View(Trips);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == id);
            return View(Trip);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var Airports = _AirportRepo.GetAsync().Result;
            var Airplanes = _AirplaneRepo.GetAsync().Result;
            var TripPlaceholder = new
            {
                Airports = Airports,
                Airplanes = Airplanes
            };
            return View(TripPlaceholder);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TripVM TripVM)
        {
            var Trip = new Trip
            {
                Price = TripVM.Price,
                DateTime = TripVM.DateTime,
                AirplaneId = TripVM.AirplaneId,
                SeatId = TripVM.SeatId,
                Airport_ToId = TripVM.Airport_ToId,
                Airport_FromId = TripVM.Airport_FromId
            };
            await _TripRepo.AddAsync(Trip);
            await _TripRepo.CommitAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == id);
            var Airports = _AirportRepo.GetAsync().Result;
            var Airplanes = _AirplaneRepo.GetAsync().Result;
            var TripPlaceholder = new
            {
                Trip = Trip,
                Airports = Airports,
                Airplanes = Airplanes
            };
            return View(TripPlaceholder);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TripVM TripVM)
        {
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == TripVM.Id);
            Trip.Price = TripVM.Price;
            Trip.DateTime = TripVM.DateTime;
            Trip.AirplaneId = TripVM.AirplaneId;
            Trip.SeatId = TripVM.SeatId;
            Trip.Airport_ToId = TripVM.Airport_ToId;
            Trip.Airport_FromId = TripVM.Airport_FromId;
            _TripRepo.Update(Trip);
            await _TripRepo.CommitAsync();
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == id);
            _TripRepo.Delete(Trip);
            await _TripRepo.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
