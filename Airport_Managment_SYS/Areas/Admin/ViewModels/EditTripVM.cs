namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditTripVM
    {
        public int Id { get; set; }
        public Trip? Trip { get; set; }
        public IEnumerable<Airport> Airports { get; set; }
        public IEnumerable<Airplane> Airplanes { get; set; }
    }
}
