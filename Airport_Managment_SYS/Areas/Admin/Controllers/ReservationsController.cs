using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
 [Area("Admin")]
 [Authorize(Roles = "Admin")]
 public class ReservationsController : Controller
 {
 private readonly IRepository<Reservation> _reservationRepo;
 private readonly IRepository<Trip> _tripRepo;
 private readonly UserManager<ApplicationUser> _userManager;

 public ReservationsController(IRepository<Reservation> reservationRepo, IRepository<Trip> tripRepo, UserManager<ApplicationUser> userManager)
 {
 _reservationRepo = reservationRepo;
 _tripRepo = tripRepo;
 _userManager = userManager;
 }

 // GET: /Admin/Reservations
 // Optional userId to filter reservations for a specific user
 public async Task<IActionResult> Index(string? userId, CancellationToken cancellationToken = default)
 {
 var includes = new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip, r => r.ApplicationUser };
 var reservations = (await _reservationRepo.GetAsync(includes: includes, cancellationToken: cancellationToken)).ToList();

 if (!string.IsNullOrEmpty(userId))
 {
 // ensure user exists before filtering
 var user = await _userManager.FindByIdAsync(userId);
 if (user == null)
 {
 return NotFound();
 }

 reservations = reservations.Where(r => r.ApplicationUserId == userId).ToList();
 }

 // total price for filtered set - convert float to decimal safely
 decimal total = reservations.Sum(r => r.Trip != null ? (decimal)r.Trip.Price :0m);
 ViewBag.TotalPrice = total;

 // safe: materialize user list for the view (may be large in real apps)
 ViewBag.Users = _userManager.Users.ToList();
 ViewBag.SelectedUserId = userId;

 return View(reservations);
 }

 // GET: /Admin/Reservations/Details?tripId=1&userId=abc
 public async Task<IActionResult> Details(int tripId, string userId, CancellationToken cancellationToken = default)
 {
 if (string.IsNullOrEmpty(userId)) return BadRequest();

 // ensure user exists
 var user = await _userManager.FindByIdAsync(userId);
 if (user == null) return NotFound();

 var reservation = await _reservationRepo.GetOneAsync(r => r.TripId == tripId && r.ApplicationUserId == userId,
 includes: new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip, r => r.ApplicationUser },
 cancellationToken: cancellationToken);

 if (reservation == null) return NotFound();

 // compute total price for this reservation (here it's simply the trip price)
 ViewBag.TotalPrice = reservation.Trip != null ? (decimal)reservation.Trip.Price :0m;

 return View(reservation);
 }
 }
}
