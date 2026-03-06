using Airport_Managment_SYS.Models;
using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditAirportVM
    {
        public int Id { get; set; }
        [MaxLength(100), MinLength(2)]
        [Required(ErrorMessage = "Airport name is required")]
        public string Name { get; set; }
        [MaxLength(100), MinLength(2)]
        public string? Description { get; set; }

        public int GovernerateStateId { get; set; }

        public IEnumerable<GovernerateState>? Governerates { get; set; }
    }
}