using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AirportController : Controller
    {
        private readonly IRepository<Airport> _airportRepository;
        private readonly IRepository<GovernerateState> _governerateState;
       public AirportController(IRepository<Airport> airportRepository, IRepository<GovernerateState> governerateState)
        {
            _airportRepository = airportRepository;
            _governerateState = governerateState;
        }
        public async Task<IActionResult> Index()
        {
            var airports=await _airportRepository.GetAsync();

            return View(airports.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var governerateStates=await _governerateState.GetAsync();

            return View(new CreateAirportVM() { GovernerateStates=governerateStates});
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAirportVM createAirportVM)
        {
            if (!ModelState.IsValid)
            {
                var governerateStates = await _governerateState.GetAsync();
createAirportVM.GovernerateStates=governerateStates;
                return View(createAirportVM);

            }

            var airport = new Airport()
            {
                Name = createAirportVM.Name,
                Description = createAirportVM.Description,
                GovernerateStateId = createAirportVM.GovernerateStateId
            };
          await  _airportRepository.AddAsync(airport);
            await _airportRepository.CommitAsync();
            TempData["Success"] = "Brand Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {

            var airport = await _airportRepository.GetOneAsync(g => g.Id == id);
            if (airport == null)
            {
                return NotFound();
            }

            var governorates = await _governerateState.GetAsync();
            EditAirportVM editAirportVM = new EditAirportVM() { Id = airport.Id,Description=airport.Description, Name = airport.Name, GovernerateStateId = airport.GovernerateStateId, Governerates = governorates };

            return View(editAirportVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditAirportVM editAirportVM)
        {
            if (!ModelState.IsValid)
            {

                var governorates = await _governerateState.GetAsync();
                editAirportVM.Governerates = governorates;
                return View(editAirportVM);
            }
            var airport = await _airportRepository.GetOneAsync(g => g.Id == editAirportVM.Id);
            if (airport == null)
            {
                return NotFound();
            }
            airport.Name = editAirportVM.Name;
            airport.GovernerateStateId = editAirportVM.GovernerateStateId;
            airport.Description= editAirportVM.Description;
            _airportRepository.Update(airport);
            await _airportRepository.CommitAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var airport = await _airportRepository.GetOneAsync(c => c.Id == id);
            _airportRepository.Delete(airport);
            await _airportRepository.CommitAsync();
            TempData["Success"] = "Airport deleted successfully.";

            return RedirectToAction("Index");
            ;
        }
    }
}
