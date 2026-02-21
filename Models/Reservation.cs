using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airport_Managment.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int TripId { get; set; }

    [Range(1, int.MaxValue)]
    public int SeatsCount { get; set; }

    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public User? User { get; set; }
    public Trip? Trip { get; set; }
}
