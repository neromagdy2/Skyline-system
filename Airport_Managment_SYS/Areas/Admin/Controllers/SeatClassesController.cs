using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{ [Area("Admin")]
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class SeatClassesController : Controller
    {
       

        private readonly IRepository<SeatClass> _seatClassRepository;

        public SeatClassesController(IRepository<SeatClass> seatClassRepository)
        {
            this._seatClassRepository = seatClassRepository;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {

           var seatClasses =await _seatClassRepository.GetAsync();
            
            int totalItems = seatClasses.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var pagedSeatClasses = seatClasses.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return View(pagedSeatClasses.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SeatClass seatClass)
        {
            if (!ModelState.IsValid)
            {
                return View(seatClass);
            }
            await _seatClassRepository.AddAsync(seatClass);
            await _seatClassRepository.CommitAsync();
            TempData["Success"] = "seatClass created successfully.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seatClass = await _seatClassRepository.GetOneAsync(c => c.Id == id);
            return View(seatClass);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SeatClass seatClass)
        {
            if (!ModelState.IsValid)
            {
                return View(seatClass);
            }
            _seatClassRepository.Update(seatClass);
            await _seatClassRepository.CommitAsync();
            TempData["Success"] = "seatClass updated successfully.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var seatClass = await _seatClassRepository.GetOneAsync(c => c.Id == id);
            _seatClassRepository.Delete(seatClass);
            await _seatClassRepository.CommitAsync();
            TempData["Success"] = "seatClass deleted successfully.";

            return RedirectToAction("Index");
            ;
        }
    }
}
