namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateAirplaneVM
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public List<Seat>? Seats { get; set; }
        public IEnumerable<SeatClass>? SeatClasses { get; set; }
    }
}
