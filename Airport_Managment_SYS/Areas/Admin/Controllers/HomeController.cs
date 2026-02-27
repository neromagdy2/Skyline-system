using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Airport_Managment_SYS.DataAccess;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<Airport> _airportRepository;
        private readonly IRepository<Airplane> _airplaneRepository;
        private readonly IRepository<ApplicationUser> _applicationUserRepository;
        private readonly IRepository<Payment> _paymentRepo;

        public HomeController(IRepository<Trip> tripRepository, IRepository<Airport> airportRepository, IRepository<Airplane> airplaneRepository, IRepository<ApplicationUser> applicationUserRepository, IRepository<Payment> paymentRepo)
        {
            _tripRepository = tripRepository;
            _airportRepository = airportRepository;
            _airplaneRepository = airplaneRepository;
            _applicationUserRepository = applicationUserRepository;
            _paymentRepo = paymentRepo;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.NoOfTrips = (await _tripRepository.GetAsync()).ToList().Count();
            ViewBag.NoOfUsers = (await _applicationUserRepository.GetAsync()).ToList().Count;
            ViewBag.NoOfAirports = (await _airportRepository.GetAsync()).ToList().Count();
            ViewBag.NoOfPlanes = (await _airplaneRepository.GetAsync()).ToList().Count();
            ViewBag.TotalPaymentMoney = (await _paymentRepo.GetAsync()).ToList().Sum(p => (double?)p.Total);

            return View();
        }
    }
}