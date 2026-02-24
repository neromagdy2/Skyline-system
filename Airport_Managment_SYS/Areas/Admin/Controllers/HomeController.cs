using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Airport_Managment_SYS.DataAccess;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbcontext _context;

        public HomeController(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.NoOfTrips = await _context.Trips.CountAsync();
            ViewBag.NoOfUsers = await _context.ApplicationUsers.CountAsync();
            ViewBag.NoOfAirports = await _context.airports.CountAsync();
            ViewBag.NoOfPlanes = await _context.Airplanes.CountAsync();
            ViewBag.TotalPaymentMoney =
                await _context.Payments.SumAsync(p => (double?)p.Total) ?? 0;

            return View();
        }
    }
}