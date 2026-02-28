using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateAirportVM
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public int GovernerateStateId { get; set; }
        public IEnumerable<GovernerateState> ?GovernerateStates { get; set; }
    }
}
