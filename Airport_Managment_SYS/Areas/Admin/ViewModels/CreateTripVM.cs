namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateTripVM
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public DateTime DateTime { get; set; }
        [DateGreaterThan(nameof(DateTime))]
        public DateTime ArrivalDateTime { get; set; }
        public int AirplaneId { get; set; }
        public int SeatId { get; set; }
        public int Airport_ToId { get; set; }
        [NotEqual(nameof(Airport_ToId), ErrorMessage = "The Distanation Airport and Deprture Airport can't be the same")]

        public int Airport_FromId { get; set; }
        public IEnumerable<Airplane> Airplanes { get; set; } = Enumerable.Empty<Airplane>();
        public IEnumerable<Airport> Airports { get; set; } = Enumerable.Empty<Airport>();
    }
}
