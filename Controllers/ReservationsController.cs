using Airport_Managment.Data;
using Airport_Managment.DTOs.Reservations;
using Airport_Managment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ReservationsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult<ViewReservationDTO>> CreateReservation([FromBody] CreateReservationDTO dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(userItem => userItem.Id == dto.UserId);
        if (user is null)
        {
            return NotFound("User not found.");
        }

        var trip = await _dbContext.Trips.FirstOrDefaultAsync(tripItem => tripItem.Id == dto.TripId);
        if (trip is null)
        {
            return NotFound("Trip not found.");
        }

        if (dto.SeatsCount > trip.AvailableSeats)
        {
            return BadRequest("Not enough available seats.");
        }

        var reservation = new Reservation
        {
            UserId = dto.UserId,
            TripId = dto.TripId,
            SeatsCount = dto.SeatsCount,
            ReservationDate = DateTime.UtcNow,
            Status = ReservationStatus.Pending,
            TotalPrice = dto.SeatsCount * trip.SeatPrice
        };

        trip.AvailableSeats -= dto.SeatsCount;

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservationsByUser), new { userId = dto.UserId }, MapToViewReservationDto(reservation, user, trip));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<ViewReservationDTO>>> GetReservationsByUser(int userId)
    {
        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == userId);
        if (!userExists)
        {
            return NotFound("User not found.");
        }

        var reservations = await _dbContext.Reservations
            .Include(reservation => reservation.User)
            .Include(reservation => reservation.Trip)
            .Where(reservation => reservation.UserId == userId)
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

        return Ok(reservations);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ViewReservationDTO>> UpdateReservation(int id, [FromBody] UpdateReservationDTO dto)
    {
        if (dto.SeatsCount is null && dto.Status is null)
        {
            return BadRequest("Provide at least one value to update (SeatsCount or Status).");
        }

        var reservation = await _dbContext.Reservations
            .Include(reservationItem => reservationItem.User)
            .Include(reservationItem => reservationItem.Trip)
            .FirstOrDefaultAsync(reservationItem => reservationItem.Id == id);

        if (reservation is null)
        {
            return NotFound("Reservation not found.");
        }

        if (reservation.Trip is null || reservation.User is null)
        {
            return BadRequest("Reservation references missing user or trip.");
        }

        var originalSeats = reservation.SeatsCount;

        if (dto.SeatsCount.HasValue)
        {
            var requestedSeats = dto.SeatsCount.Value;
            var seatsDelta = requestedSeats - originalSeats;

            if (seatsDelta > 0 && reservation.Trip.AvailableSeats < seatsDelta)
            {
                return BadRequest("Not enough available seats for the update.");
            }

            reservation.Trip.AvailableSeats -= seatsDelta;
            reservation.SeatsCount = requestedSeats;
        }

        if (dto.Status.HasValue)
        {
            reservation.Status = dto.Status.Value;
        }

        reservation.TotalPrice = reservation.SeatsCount * reservation.Trip.SeatPrice;

        await _dbContext.SaveChangesAsync();

        return Ok(MapToViewReservationDto(reservation, reservation.User, reservation.Trip));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _dbContext.Reservations
            .Include(reservationItem => reservationItem.Trip)
            .FirstOrDefaultAsync(reservationItem => reservationItem.Id == id);

        if (reservation is null)
        {
            return NotFound("Reservation not found.");
        }

        if (reservation.Trip is null)
        {
            return BadRequest("Reservation references missing trip.");
        }

        reservation.Trip.AvailableSeats += reservation.SeatsCount;

        _dbContext.Reservations.Remove(reservation);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static ViewReservationDTO MapToViewReservationDto(Reservation reservation, User user, Trip trip)
    {
        return new ViewReservationDTO
        {
            Id = reservation.Id,
            UserId = reservation.UserId,
            UserName = user.FullName,
            TripId = reservation.TripId,
            TripRoute = trip.Route,
            SeatsCount = reservation.SeatsCount,
            ReservationDate = reservation.ReservationDate,
            Status = reservation.Status,
            TotalPrice = reservation.TotalPrice
        };
    }
}
