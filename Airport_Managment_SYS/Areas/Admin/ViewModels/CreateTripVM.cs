namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateTripVM
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public DateTime DateTime { get; set; }
        public int AirplaneId { get; set; }
        public int SeatId { get; set; }
        public int Airport_ToId { get; set; }
        public int Airport_FromId { get; set; }
        public IEnumerable<Airplane> Airplanes { get; set; }
        public IEnumerable<Airport> Airports { get; set; }
    }
}
