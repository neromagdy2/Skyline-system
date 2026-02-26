namespace Airport_Managment_SYS.Models
{
    public class Airplane
    {
        public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Model { get; set; } = null!;
    public IEnumerable<Seat> Seats { get; set; } = new List<Seat>();
    }
}
