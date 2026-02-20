    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
using Airport_Managment_SYS.Areas.Admin.ViewModels;


    namespace Airport_Managment_SYS.Areas.Admin.Controllers
    {
        [Area("Admin")]
        public class TripController : Controller
        {           
            private IRepository<Trip> _TripRepo;
            private IRepository<Airplane> _AirplaneRepo;
            private IRepository<Airport> _AirportRepo;
            public TripController(IRepository<Trip> TripRepo, IRepository<Airplane> airplaneRepo, IRepository<Airport> airportRepo)
            {
                _TripRepo = TripRepo;
                _AirplaneRepo = airplaneRepo;
                _AirportRepo = airportRepo;
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var Trips = (await _TripRepo.GetAsync()).ToList();

                return View(Trips);
            }
            [HttpGet]
            public async Task<IActionResult> Details(int id)
            {
                var Trip = await _TripRepo.GetOneAsync(t => t.Id == id);
                return View(Trip);
            }
            [HttpGet]
            public async Task<IActionResult> Create()
            {
                var Airports = await _AirportRepo.GetAsync();
                var Airplanes = await _AirplaneRepo.GetAsync();
                var TripPlaceholder = new CreateTripVM
                {
                    Airports = Airports,
                    Airplanes = Airplanes
                };
                return View(TripPlaceholder);
            }
            [HttpPost]
            public async Task<IActionResult> Create(CreateTripVM TripVM)
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
                var Airports = await _AirportRepo.GetAsync();
                var Airplanes = await _AirplaneRepo.GetAsync();
                var TripPlaceholder = new EditTripVM
                {
                    Trip = Trip,
                    Airports = Airports,
                    Airplanes = Airplanes
                };
                return View(TripPlaceholder);
            }
            [HttpPost]
            public async Task<IActionResult> Edit(EditTripVM TripVM)
            {
                var Trip = await _TripRepo.GetOneAsync(t => t.Id == TripVM.Id);
                Trip.Price = TripVM.Trip.Price;
                Trip.DateTime = TripVM.Trip.DateTime;
                Trip.AirplaneId = TripVM.Trip.AirplaneId;
                Trip.SeatId = TripVM.Trip.SeatId;
                Trip.Airport_ToId = TripVM.Trip.Airport_ToId;
                Trip.Airport_FromId = TripVM.Trip.Airport_FromId;
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
