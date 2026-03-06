using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditAirplaneVM
    {
        public int Id { get; set; }
        [MaxLength(100), MinLength(2)]
        [Required(ErrorMessage = "Airplane name is required")]
        public string Name { get; set; }
        [MaxLength(100), MinLength(2)]
        public string Model { get; set; }
        public List<Seat>? Seats { get; set; }= new List<Seat>();
        public IEnumerable<SeatClass>? SeatClasses { get; set; }
    }
}
