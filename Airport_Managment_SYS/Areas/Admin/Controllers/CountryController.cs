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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var countries =await _countryRepository.GetAsync();
            
            int totalItems = countries.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var pagedCountries = countries.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return View(pagedCountries.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
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
