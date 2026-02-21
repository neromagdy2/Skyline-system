using Airport_Managment.Models;

namespace Airport_Managment.DTOs.Reservations;

public class ViewReservationDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TripId { get; set; }
    public string TripRoute { get; set; } = string.Empty;
    public int SeatsCount { get; set; }
    public DateTime ReservationDate { get; set; }
    public ReservationStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
}
