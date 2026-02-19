using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Areas.Identity.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
