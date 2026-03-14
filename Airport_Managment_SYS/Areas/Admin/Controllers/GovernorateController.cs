using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "SuperAdmin , Admin")]
    public class GovernorateController : Controller
    {
        private readonly IRepository<GovernerateState> _governerateState;
        private readonly IRepository<Country> _countryRepository;
        public GovernorateController(IRepository<GovernerateState> governerateState, IRepository<Country> countryRepository)
        {
            _governerateState = governerateState;
            _countryRepository = countryRepository;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {

            var governerateStates = await _governerateState.GetAsync();
            
            int totalItems = governerateStates.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var pagedGovernerateStates = governerateStates.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return View(pagedGovernerateStates.AsEnumerable());
        }
 
        public async Task<IActionResult> Create()
        {
            var countries = await _countryRepository.GetAsync();
            return View(new CreateGovernorateVM() { Countries=countries});
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateGovernorateVM createGovernorateVM)
        {
            if (!ModelState.IsValid)
            {
                var countries = await _countryRepository.GetAsync();
                createGovernorateVM.Countries = countries;
                return View(createGovernorateVM);
            }
         await  _governerateState.AddAsync(new GovernerateState() { Name = createGovernorateVM.Name, CountryId = createGovernorateVM.CountryId });
           await _governerateState.CommitAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {

            var governerateState = await _governerateState.GetOneAsync(g=>g.Id==id);
                if (governerateState == null)
                {
                    return NotFound();
            }

                var countries = await _countryRepository.GetAsync();
            EditGovernrateVM editGovernrateVM = new EditGovernrateVM() { Id = governerateState.Id, Name = governerateState.Name, CountryId = governerateState.CountryId ,Countries=countries};  

            return View(editGovernrateVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditGovernrateVM editGovernrateVM)
        {
           if(!ModelState.IsValid)
            {
                var countries = await _countryRepository.GetAsync();
                editGovernrateVM.Countries = countries;
                return View(editGovernrateVM);
            }
            var governerateState = await _governerateState.GetOneAsync(g => g.Id == editGovernrateVM.Id);
            if (governerateState == null)
            {
                return NotFound();
            }
            governerateState.Name = editGovernrateVM.Name;
            governerateState.CountryId = editGovernrateVM.CountryId;
              _governerateState.Update(governerateState);
           await _governerateState.CommitAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var governerate = await _governerateState.GetOneAsync(c => c.Id == id);
            _governerateState.Delete(governerate);
            await _countryRepository.CommitAsync();
            TempData["Success"] = "governorate deleted successfully.";

            return RedirectToAction("Index");
            ;
        }
    }
}
