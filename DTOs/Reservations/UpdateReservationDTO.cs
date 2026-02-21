using System.ComponentModel.DataAnnotations;
using Airport_Managment.Models;

namespace Airport_Managment.DTOs.Reservations;

public class UpdateReservationDTO
{
    [Range(1, int.MaxValue)]
    public int? SeatsCount { get; set; }

    public ReservationStatus? Status { get; set; }
}
