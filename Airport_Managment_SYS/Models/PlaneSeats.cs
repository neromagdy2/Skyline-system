namespace Airport_Managment_SYS.Models
{
    public class PlaneSeats
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AirplaneId { get; set; }
        public Airplane ?Plane { get; set; }
        public int SeatClassId { get; set; }
        public SeatClass ?SeatClass { get; set; }

    }
}
