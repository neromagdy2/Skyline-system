using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AirplaneController : Controller
    {

        private readonly IRepository<Airplane> _airplaneRepository;
        private readonly IRepository<Seat> _seatsRepository;
        private readonly IRepository<SeatClass> _seatClassesRepository;
        public AirplaneController(IRepository<Airplane> airplaneRepository, IRepository<Seat> seatsRepository, IRepository<SeatClass> seatClassesRepository)
        {
            _airplaneRepository = airplaneRepository;
            _seatsRepository = seatsRepository;
            _seatClassesRepository = seatClassesRepository;
        }

        public async Task<IActionResult> Index()
        {

            var airplanes = await _airplaneRepository.GetAsync();
            return View(airplanes.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var seatClasses = await _seatClassesRepository.GetAsync();
            return View(new CreateAirplaneVM() { SeatClasses = seatClasses });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAirplaneVM createAirplaneVM)
        {
            if (!ModelState.IsValid)
            {
                var seatClasses = await _seatClassesRepository.GetAsync();
                createAirplaneVM.SeatClasses = seatClasses;
                return View(createAirplaneVM);
            }

            var airplane = new Airplane()
            {
                Name = createAirplaneVM.Name,
                Model = createAirplaneVM.Model,
            };
            await _airplaneRepository.AddAsync(airplane);
            await _airplaneRepository.CommitAsync();


            if (createAirplaneVM.Seats != null)

            {
                foreach (var seat in createAirplaneVM.Seats)
                {
                    await _seatsRepository.AddAsync(
                        new Seat()
                        {
                            SeatNumber = seat.SeatNumber,
                            AirplaneId = airplane.Id,
                            seatClassId = seat.seatClassId,
                            Available = seat.Available,
                            Price = seat.Price
                        });
                    await _seatsRepository.CommitAsync();
                }
            }
            TempData["Success"] = "Airplane created successfully.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var planeSeats = await _seatsRepository.GetAsync(p=>p.AirplaneId==id);
            var seatClasses = await _seatClassesRepository.GetAsync();
            var airplane = await _airplaneRepository.GetOneAsync(a => a.Id == id);
            if(airplane == null)
            {
                return NotFound();
            }
            return View(new EditAirplaneVM() {Name=airplane.Name,Model=airplane.Model, Id=airplane.Id,SeatClasses = seatClasses,Seats= planeSeats.ToList() });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditAirplaneVM EditAirplaneVM)
        {
            if (!ModelState.IsValid)
            {
                var seatClasses = await _seatClassesRepository.GetAsync();
                EditAirplaneVM.SeatClasses = seatClasses;
                return View(EditAirplaneVM);
            }
          
            Airplane airplane =await _airplaneRepository.GetOneAsync(a=>a.Id==EditAirplaneVM.Id); 
            if (airplane == null)
            {
                return NotFound();
            }
            airplane.Name = EditAirplaneVM.Name;
            airplane.Model = EditAirplaneVM.Model;
                _airplaneRepository.Update(airplane);
                await _airplaneRepository.CommitAsync();
            if (EditAirplaneVM.Seats != null)
            {
                var planeSeats = await _seatsRepository.GetAsync(p => p.AirplaneId == EditAirplaneVM.Id);
                if (planeSeats.Any())
                {
                    foreach (var seat in planeSeats)
                    {
                        _seatsRepository.Delete(seat);
                        await _seatsRepository.CommitAsync();
                    }
                }
                foreach (var seat in EditAirplaneVM.Seats)
                {
                    await _seatsRepository.AddAsync(
                        new Seat()
                        {
                            SeatNumber = seat.SeatNumber,
                            AirplaneId = airplane.Id,
                            seatClassId = seat.seatClassId,
                            Available = seat.Available,
                            Price = seat.Price
                        });
                    await _seatsRepository.CommitAsync();
                }
            }


            await _airplaneRepository.CommitAsync();


            if (EditAirplaneVM.Seats != null)

            {
                foreach (var seat in EditAirplaneVM.Seats)
                {
                    await _seatsRepository.AddAsync(
                        new Seat()
                        {
                            SeatNumber = seat.SeatNumber,
                            AirplaneId = airplane.Id,
                            seatClassId = seat.seatClassId,
                            Available = seat.Available,
                            Price = seat.Price
                        });
                    await _seatsRepository.CommitAsync();
                }
            }
            TempData["Success"] = "Airplane created successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {

            var airplane = await _airplaneRepository.GetOneAsync(a => a.Id == id);
            var seats = await _seatsRepository.GetAsync(s => s.AirplaneId == id);
            if (seats.Any())
            {
                foreach (var seat in seats)
                {
                    _seatsRepository.Delete(seat);
                    await _seatsRepository.CommitAsync();
                }
            }
            if (airplane == null)
            {
                return NotFound();
            }
            _airplaneRepository.Delete(airplane);
            await _airplaneRepository.CommitAsync();
            return RedirectToAction("Index");
        }

    }
}
