using System.Collections.Generic;
using System.Linq;
using Airport_Managment_SYS.Models;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditTripVM
    {
        public int Id { get; set; }
        public Trip? Trip { get; set; }
        public IEnumerable<Airport> Airports { get; set; } = Enumerable.Empty<Airport>();
        public IEnumerable<Airplane> Airplanes { get; set; } = Enumerable.Empty<Airplane>();
    }
}
