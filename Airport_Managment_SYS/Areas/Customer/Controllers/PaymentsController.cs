using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
