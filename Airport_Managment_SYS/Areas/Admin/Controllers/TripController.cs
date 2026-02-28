using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Airport_Managment_SYS.Repositories;
using Airport_Managment_SYS.Models;


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
            var Trips = (await _TripRepo.GetAsync(t=>t.IsDeleted != true)).ToList();

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
                Airplanes = Airplanes,
                DateTime = DateTime.Now
            };
            return View(TripPlaceholder);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTripVM TripVM)
        {
            if (!ModelState.IsValid)
            {
                // repopulate lists for the view
                TripVM.Airports = await _AirportRepo.GetAsync();
                TripVM.Airplanes = await _AirplaneRepo.GetAsync();
                return View(TripVM);
            }
            var Trip = new Trip
            {
                Price = TripVM.Price,
                DateTime = TripVM.DateTime,
                AirplaneId = TripVM.AirplaneId,
                Airport_ToId = TripVM.Airport_ToId,
                Airport_FromId = TripVM.Airport_FromId
            };

            var plane = await _AirplaneRepo.GetOneAsync(
                p => p.Id == TripVM.AirplaneId,
                includes: [p=>p.Seats]);

            if (plane == null)
                return View(Trip);

            // copy airplane seats into trip seats
            foreach (var seat in plane.Seats)
            {
                Trip.TripSeats.Add(new TripSeat
                {
                    SeatId = seat.Id,
                    IsBooked = false
                });
            }

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

            if (!ModelState.IsValid)
            {
                return View(TripVM);
            }
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == TripVM.Id);

            if (Trip == null)
                return RedirectToAction("Index");
            var vmTrip = TripVM.Trip;
            if (vmTrip == null)
            {
                // repopulate lists and show view
                TripVM.Airports = await _AirportRepo.GetAsync();
                TripVM.Airplanes = await _AirplaneRepo.GetAsync();
                return View(TripVM);
            }

            Trip.Price = vmTrip.Price;
            Trip.DateTime = vmTrip.DateTime;
            Trip.AirplaneId = vmTrip.AirplaneId;
            Trip.Airport_ToId = vmTrip.Airport_ToId;
            Trip.Airport_FromId = vmTrip.Airport_FromId;
            _TripRepo.Update(Trip);
            await _TripRepo.CommitAsync();
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == id);

            if(Trip == null)
                return RedirectToAction("Index");

            Trip.IsDeleted = true;
            await _TripRepo.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
