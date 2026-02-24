using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
 [Area("Admin")]
 [Authorize(Roles = "Admin")]
 public class ReservationController : Controller
 {
 private readonly IRepository<Reservation> _reservationRepo;
 private readonly IRepository<Trip> _tripRepo;
 private readonly UserManager<ApplicationUser> _userManager;

 public ReservationController(IRepository<Reservation> reservationRepo, IRepository<Trip> tripRepo, UserManager<ApplicationUser> userManager)
 {
 _reservationRepo = reservationRepo;
 _tripRepo = tripRepo;
 _userManager = userManager;
 }

 // View all user reservations
 public async Task<IActionResult> Index()
 {
 var reservations = (await _reservationRepo.GetAsync(
 includes: new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip, r => r.ApplicationUser }))
 .ToList();

 return View(reservations);
 }

 // Create reservation - GET
 [HttpGet]
 public async Task<IActionResult> Create()
 {
 var trips = (await _tripRepo.GetAsync()).ToList();
 var users = _userManager.Users.ToList();
 ViewBag.Trips = trips;
 ViewBag.Users = users;
 return View();
 }

 // Create reservation - POST
 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> Create(int tripId, string userId)
 {
 if (string.IsNullOrEmpty(userId))
 {
 ModelState.AddModelError(string.Empty, "User is required.");
 }

 var trip = await _tripRepo.GetOneAsync(t => t.Id == tripId);
 if (trip == null)
 {
 ModelState.AddModelError(string.Empty, "Selected trip does not exist.");
 }

 var duplicate = await _reservationRepo.GetOneAsync(r => r.TripId == tripId && r.ApplicationUserId == userId);
 if (duplicate != null)
 {
 ModelState.AddModelError(string.Empty, "This reservation already exists for the selected user and trip.");
 }

 if (!ModelState.IsValid)
 {
 var trips = (await _tripRepo.GetAsync()).ToList();
 var users = _userManager.Users.ToList();
 ViewBag.Trips = trips;
 ViewBag.Users = users;
 return View();
 }

 var reservation = new Reservation
 {
 TripId = tripId,
 ApplicationUserId = userId
 };

 await _reservationRepo.AddAsync(reservation);
 await _reservationRepo.CommitAsync();

 return RedirectToAction(nameof(Index));
 }

 // Edit reservation - GET
 public async Task<IActionResult> Edit(int tripId, string userId)
 {
 if (string.IsNullOrEmpty(userId)) return BadRequest();

 var reservation = await _reservationRepo.GetOneAsync(r => r.TripId == tripId && r.ApplicationUserId == userId,
 includes: new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip, r => r.ApplicationUser });

 if (reservation == null) return NotFound();

 var trips = (await _tripRepo.GetAsync()).ToList();

 ViewBag.Trips = trips;

 return View(reservation);
 }

 // Edit reservation - POST
 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> Edit(int originalTripId, string originalUserId, int newTripId)
 {
 if (string.IsNullOrEmpty(originalUserId)) return BadRequest();

 var existing = await _reservationRepo.GetOneAsync(r => r.TripId == originalTripId && r.ApplicationUserId == originalUserId);
 if (existing == null) return NotFound();

 // If nothing changed, just redirect
 if (originalTripId == newTripId)
 {
 return RedirectToAction(nameof(Index));
 }

 // ensure new trip exists
 var newTrip = await _tripRepo.GetOneAsync(t => t.Id == newTripId);
 if (newTrip == null)
 {
 ModelState.AddModelError(string.Empty, "Selected trip does not exist.");
 }

 // prevent duplicate reservation
 var duplicate = await _reservationRepo.GetOneAsync(r => r.TripId == newTripId && r.ApplicationUserId == originalUserId);
 if (duplicate != null)
 {
 ModelState.AddModelError(string.Empty, "A reservation for this user and trip already exists.");
 }

 if (!ModelState.IsValid)
 {
 var trips = (await _tripRepo.GetAsync()).ToList();
 ViewBag.Trips = trips;

 var reservation = await _reservationRepo.GetOneAsync(r => r.TripId == originalTripId && r.ApplicationUserId == originalUserId,
 includes: new System.Linq.Expressions.Expression<System.Func<Reservation, object>>[] { r => r.Trip, r => r.ApplicationUser });

 return View(reservation);
 }

 // remove old reservation and add new one for same user
 _reservationRepo.Delete(existing);

 var newReservation = new Reservation
 {
 TripId = newTripId,
 ApplicationUserId = originalUserId
 };

 await _reservationRepo.AddAsync(newReservation);
 await _reservationRepo.CommitAsync();

 return RedirectToAction(nameof(Index));
 }

 // Delete reservation
 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> Delete(int tripId, string userId)
 {
 if (string.IsNullOrEmpty(userId)) return BadRequest();

 var reservation = await _reservationRepo.GetOneAsync(r => r.TripId == tripId && r.ApplicationUserId == userId);
 if (reservation == null) return NotFound();

 _reservationRepo.Delete(reservation);
 await _reservationRepo.CommitAsync();

 return RedirectToAction(nameof(Index));
 }
 }
}
