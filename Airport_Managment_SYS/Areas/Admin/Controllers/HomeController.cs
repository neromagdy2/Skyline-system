using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{[Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
