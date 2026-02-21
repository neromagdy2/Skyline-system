using Airport_Managment.Data;
using Airport_Managment.DTOs.Reservations;
using Airport_Managment.Models;
using Airport_Managment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment.Controllers;

public class ReservationDashboardController : Controller
{
    private readonly AppDbContext _dbContext;

    public ReservationDashboardController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index(int? userId)
    {
        try
        {
            var query = _dbContext.Reservations
                .AsNoTracking()
                .Include(reservation => reservation.User)
                .Include(reservation => reservation.Trip)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(reservation => reservation.UserId == userId.Value);
            }

            var reservations = await query
                .OrderByDescending(reservation => reservation.ReservationDate)
                .Select(reservation => new ViewReservationDTO
                {
                    Id = reservation.Id,
                    UserId = reservation.UserId,
                    UserName = reservation.User != null ? reservation.User.FullName : string.Empty,
                    TripId = reservation.TripId,
                    TripRoute = reservation.Trip != null ? reservation.Trip.Route : string.Empty,
                    SeatsCount = reservation.SeatsCount,
                    ReservationDate = reservation.ReservationDate,
                    Status = reservation.Status,
                    TotalPrice = reservation.TotalPrice
                })
                .ToListAsync();

            var model = new ReservationDashboardViewModel
            {
                SelectedUserId = userId,
                TotalReservations = reservations.Count,
                PendingReservations = reservations.Count(reservation => reservation.Status == ReservationStatus.Pending),
                ConfirmedReservations = reservations.Count(reservation => reservation.Status == ReservationStatus.Confirmed),
                CancelledReservations = reservations.Count(reservation => reservation.Status == ReservationStatus.Cancelled),
                TotalRevenue = reservations.Where(reservation => reservation.Status != ReservationStatus.Cancelled).Sum(reservation => reservation.TotalPrice),
                Reservations = reservations
            };

            return View(model);
        }
        catch (Exception)
        {
            return View(new ReservationDashboardViewModel
            {
                SelectedUserId = userId,
                LoadErrorMessage = "Unable to load reservations currently. Check database connection and run migrations."
            });
        }
    }
}
