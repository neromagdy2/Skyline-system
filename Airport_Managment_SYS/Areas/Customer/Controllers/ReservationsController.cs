using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
 [Area("Customer")]
 [Authorize]
 public class ReservationsController : Controller
 {
 private readonly IRepository<Reservation> _reservationRepo;
 private readonly UserManager<ApplicationUser> _userManager;

 public ReservationsController(IRepository<Reservation> reservationRepo, UserManager<ApplicationUser> userManager)
 {
 _reservationRepo = reservationRepo;
 _userManager = userManager;
 }

 // GET: /Customer/Reservations
 public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
 {
 var userId = _userManager.GetUserId(User);
 if (string.IsNullOrEmpty(userId))
 {
 return Challenge();
 }

 var includes = new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip };
 var reservations = (await _reservationRepo.GetAsync(r => r.ApplicationUserId == userId, includes: includes, cancellationToken: cancellationToken)).ToList();

 decimal total = reservations.Sum(r => r.Trip != null ? (decimal)r.Trip.Price :0m);
 ViewBag.TotalPrice = total;

 return View(reservations);
 }

 // GET: /Customer/Reservations/Details?tripId=1
 public async Task<IActionResult> Details(int tripId, CancellationToken cancellationToken = default)
 {
 var userId = _userManager.GetUserId(User);
 if (string.IsNullOrEmpty(userId)) return Challenge();

 var reservation = await _reservationRepo.GetOneAsync(r => r.TripId == tripId && r.ApplicationUserId == userId,
 includes: new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip },
 cancellationToken: cancellationToken);

 if (reservation == null) return NotFound();

 ViewBag.TotalPrice = reservation.Trip != null ? (decimal)reservation.Trip.Price :0m;
 return View(reservation);
 }
 }
}
