namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditAirplaneVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public List<Seat>? Seats { get; set; }= new List<Seat>();
        public IEnumerable<SeatClass>? SeatClasses { get; set; }
    }
}
