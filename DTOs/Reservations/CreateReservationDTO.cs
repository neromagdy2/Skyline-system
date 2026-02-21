using System.ComponentModel.DataAnnotations;

namespace Airport_Managment.DTOs.Reservations;

public class CreateReservationDTO
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int TripId { get; set; }

    [Range(1, int.MaxValue)]
    public int SeatsCount { get; set; }
}
