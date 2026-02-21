namespace Airport_Managment.Models;

public class Trip
{
    public int Id { get; set; }
    public string Route { get; set; } = string.Empty;
    public decimal SeatPrice { get; set; }
    public int AvailableSeats { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
