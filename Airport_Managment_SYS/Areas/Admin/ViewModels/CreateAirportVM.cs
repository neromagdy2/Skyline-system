using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateAirportVM
    {
        [Required]
        [MaxLength(100), MinLength(2)]

        public string Name { get; set; }
        [MaxLength(100), MinLength(2)]
        public string? Description { get; set; }
        public int GovernerateStateId { get; set; }
        public IEnumerable<GovernerateState> ?GovernerateStates { get; set; }
    }
}
