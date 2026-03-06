using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateAirplaneVM
    {

        [MaxLength(100),MinLength(2)]
        [Required(ErrorMessage = "Airplane name is required")]
        public string Name { get; set; }

        [MaxLength(100), MinLength(2)]
        public string Model { get; set; }
        public List<Seat>? Seats { get; set; }
        public IEnumerable<SeatClass>? SeatClasses { get; set; }
    }
}
