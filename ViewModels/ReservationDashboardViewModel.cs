using Airport_Managment.DTOs.Reservations;

namespace Airport_Managment.ViewModels;

public class ReservationDashboardViewModel
{
    public int? SelectedUserId { get; set; }
    public string? LoadErrorMessage { get; set; }
    public int TotalReservations { get; set; }
    public int PendingReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CancelledReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ViewReservationDTO> Reservations { get; set; } = new();
}
