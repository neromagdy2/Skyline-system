using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TripDetails()
        {
            return View();
        }
    }
}
