using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Airport_Managment_SYS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateTripVM
    {
        [Range(25.00, float.MaxValue, ErrorMessage = "Minimum trip price is 25.00 EGP to meet payment processing requirements.")]
        public float Price { get; set; }
        public DateTime DateTime { get; set; }
        [DateGreaterThan(nameof(DateTime))]
        public DateTime ArrivalDateTime { get; set; }
        public int AirplaneId { get; set; }
        public int Airport_ToId { get; set; }
        [NotEqual(nameof(Airport_ToId), ErrorMessage = "The Distanation Airport and Deprture Airport can't be the same")]
        public int Airport_FromId { get; set; }

        [ValidateNever]
        public IEnumerable<Airplane> Airplanes { get; set; } = Enumerable.Empty<Airplane>();

        [ValidateNever]
        public IEnumerable<Airport> Airports { get; set; } = Enumerable.Empty<Airport>();
    }
}
