using Airport_Managment_SYS.Models;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditAirportVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int GovernerateStateId { get; set; }

        public IEnumerable<GovernerateState>? Governerates { get; set; }
    }
}