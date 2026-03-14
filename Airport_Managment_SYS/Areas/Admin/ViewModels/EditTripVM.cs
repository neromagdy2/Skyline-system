using System.Collections.Generic;
using System.Linq;
using Airport_Managment_SYS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditTripVM
    {
        public int Id { get; set; }
        public Trip? Trip { get; set; }
        [ValidateNever]
        public IEnumerable<Airport> Airports { get; set; } = Enumerable.Empty<Airport>();
        [ValidateNever]
        public IEnumerable<Airplane> Airplanes { get; set; } = Enumerable.Empty<Airplane>();
    }
}
