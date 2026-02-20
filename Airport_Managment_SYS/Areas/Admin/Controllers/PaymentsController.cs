using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
