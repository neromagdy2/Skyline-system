using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class CountryController : Controller
    {
        private readonly IRepository<Country> _countryRepository;
        public CountryController(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var countries =await _countryRepository.GetAsync();
            return View(countries.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Country country)
        {
            if (!ModelState.IsValid)
            {
                return View(country);
            }
            await _countryRepository.AddAsync(country);
            await _countryRepository.CommitAsync();
            TempData["Success"] = "Country created successfully.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var country = await _countryRepository.GetOneAsync(c => c.Id == id);
            return View(country);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Country country)
        {
            if (!ModelState.IsValid)
            {
                return View(country);
            }
            _countryRepository.Update(country);
            await _countryRepository.CommitAsync();
            TempData["Success"] = "Country updated successfully.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var country = await _countryRepository.GetOneAsync(c => c.Id == id);
            if (country != null)
            {
                _countryRepository.Delete(country);
                await _countryRepository.CommitAsync();
                TempData["Success"] = "Country deleted successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
