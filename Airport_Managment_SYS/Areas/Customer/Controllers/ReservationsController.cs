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

        // GET: /Customer/Reservations - Redirected to Bookings
        public IActionResult Index()
        {
            return RedirectToAction("Bookings", "Home", new { area = "Customer" });
        }

        // GET: /Customer/Reservations/Details - Redirected to Bookings
        public IActionResult Details(int tripId)
        {
            return RedirectToAction("Bookings", "Home", new { area = "Customer" });
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
