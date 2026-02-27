using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{ [Area("Admin")]
    public class SeatClassesController : Controller
    {
       

        private readonly IRepository<SeatClass> _seatClassRepository;

        public SeatClassesController(IRepository<SeatClass> seatClassRepository)
        {
            this._seatClassRepository = seatClassRepository;
        }

        public async Task<IActionResult> Index()
        {

           var seatClasses =await _seatClassRepository.GetAsync();
            return View(seatClasses.AsEnumerable());
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
