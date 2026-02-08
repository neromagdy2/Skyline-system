namespace Airport_Managment_SYS.Models
{
    public class Airplane
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model{ get; set; }
        public IEnumerable<Seat> Seats { get; set; }
    }
}
