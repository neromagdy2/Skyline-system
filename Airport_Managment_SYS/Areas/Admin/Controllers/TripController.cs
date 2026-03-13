using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin , Admin")]
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
        public async Task<IActionResult> Index(int page = 1, string fromSearch = "", string toSearch = "", DateTime? dateSearch = null, int pageSize = 10)
        {
            // Get trips with includes
            var query = await _TripRepo.GetAsync(
                t => t.IsDeleted != true,
                includes: new Expression<Func<Trip, object>>[]
                {
                    t => t.Airport_From,
                    t => t.Airport_From.GovernerateState,
                    t => t.Airport_To,
                    t => t.Airport_To.GovernerateState,
                    t => t.Airplane
                });
            
            // Apply search filter
            if (!string.IsNullOrEmpty(fromSearch))
            {
                var q = fromSearch.ToLower().Trim();
                query = query.Where(t => t.Airport_From != null && 
                                      (t.Airport_From.Name.ToLower().Contains(q) || 
                                      (t.Airport_From.GovernerateState != null && t.Airport_From.GovernerateState.Name.ToLower().Contains(q))));
            }

            if (!string.IsNullOrEmpty(toSearch))
            {
                var q = toSearch.ToLower().Trim();
                query = query.Where(t => t.Airport_To != null && 
                                      (t.Airport_To.Name.ToLower().Contains(q) || 
                                      (t.Airport_To.GovernerateState != null && t.Airport_To.GovernerateState.Name.ToLower().Contains(q))));
            }

            if (dateSearch.HasValue)
            {
                query = query.Where(t => t.DateTime.Date == dateSearch.Value.Date);
            }
            
            // Apply sorting for better performance
            query = query.OrderByDescending(t => t.DateTime);
            
            // Apply pagination
            var totalItems = query.Count();
            var trips = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.FromSearch = fromSearch;
            ViewBag.ToSearch = toSearch;
            ViewBag.DateSearch = dateSearch;

            return View(trips);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var Trip = await _TripRepo.GetOneAsync(t => t.Id == id);
            return View(Trip);
        }


        //Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var Airports = await _AirportRepo.GetAsync();
            var Airplanes = await _AirplaneRepo.GetAsync();
            var TripPlaceholder = new CreateTripVM
            {
                Airports = Airports,
                Airplanes = Airplanes,
                DateTime = DateTime.Now,
                ArrivalDateTime = DateTime.Now.AddHours(3)
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
            if (TripVM.DateTime < DateTime.Now)
            {
                ModelState.AddModelError("TripVM.DateTime", "The trip date cannot be in the past.");

                TripVM.Airports = await _AirportRepo.GetAsync();
                TripVM.Airplanes = await _AirplaneRepo.GetAsync();
                return View(TripVM);
            }
            var Trip = new Trip
            {
                Price = TripVM.Price,
                DateTime = TripVM.DateTime,
                ArrivalDateTime = TripVM.ArrivalDateTime,
                AirplaneId = TripVM.AirplaneId,
                Airport_ToId = TripVM.Airport_ToId,
                Airport_FromId = TripVM.Airport_FromId,
                TripSeats = new List<TripSeat>()
            };

            var plane = await _AirplaneRepo.GetOneAsync(
                p => p.Id == TripVM.AirplaneId,
                includeFunc: q => q.Include(p => p.Seats)
            );

            if (plane == null)
                return View(Trip);

            // copy airplane seats into trip seats (plane.Seats may be null if no seats were configured)
            foreach (var seat in plane.Seats ?? Enumerable.Empty<Seat>())
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



        //Edit/Update
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
                TripVM.Airports = await _AirportRepo.GetAsync();
                TripVM.Airplanes = await _AirplaneRepo.GetAsync();
                return View(TripVM);
            }

            if (TripVM.Trip.DateTime < DateTime.Now && !User.IsInRole("SuperAdmin"))
            {
                ModelState.AddModelError("Trip.DateTime", "You don't have acces to make the date in the past.");

                TripVM.Airports = await _AirportRepo.GetAsync();
                TripVM.Airplanes = await _AirplaneRepo.GetAsync();
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
            Trip.ArrivalDateTime = vmTrip.ArrivalDateTime;
            Trip.AirplaneId = vmTrip.AirplaneId;
            Trip.Airport_ToId = vmTrip.Airport_ToId;
            Trip.Airport_FromId = vmTrip.Airport_FromId;
            _TripRepo.Update(Trip);
            await _TripRepo.CommitAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
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
