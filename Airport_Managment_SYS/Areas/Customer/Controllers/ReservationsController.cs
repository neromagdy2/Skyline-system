using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;
using Airport_Managment_SYS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IRepository<Reservation> _reservationRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketService _ticketService;

        public ReservationsController(IRepository<Reservation> reservationRepo, UserManager<ApplicationUser> userManager, ITicketService ticketService)
        {
            _reservationRepo = reservationRepo;
            _userManager = userManager;
            _ticketService = ticketService;
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

        // GET: /Customer/Reservations/DownloadTicket/{id}
        [Route("Customer/Reservation/DownloadTicket/{id}")]
        public async Task<IActionResult> DownloadTicket(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var reservation = await _reservationRepo.GetOneAsync(r => r.Id == id && r.ApplicationUserId == userId,
                includeFunc: q => q
                    .Include(r => r.ReservationSeats)
                        .ThenInclude(rs => rs.Seat)
                            .ThenInclude(rss => rss.SeatClass)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_From)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_To)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airplane));

            if (reservation == null) return NotFound();

            if (!reservation.IsPaid)
            {
                TempData["Error"] = "Ticket download is only available for paid reservations.";
                return RedirectToAction("Index");
            }

            try
            {
                var ticketPdf = await _ticketService.GenerateTicketPdf(reservation);
                var ticketFileName = $"SKYSTREAM_Ticket_{reservation.Id:D6}.pdf";
                
                return File(ticketPdf, "application/pdf", ticketFileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "There was an error generating your ticket. Please try again later.";
                return RedirectToAction("Index");
            }
        }
 }
}
