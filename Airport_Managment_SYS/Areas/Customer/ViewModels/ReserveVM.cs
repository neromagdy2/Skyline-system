using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Airport_Managment_SYS.Areas.Customer.ViewModels
{
    public class ReserveVM
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        public int SeatClassId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one seat must be selected")]
        public List<int> SeatIds { get; set; } = new List<int>();

        public decimal TotalPrice { get; set; }



        ///// passenger reserve data collection
        ///
        //public string PassengerName { get; set; }

        //public string PassengerNationality { get; set; }

        //public string PassengerPassportNumber { get; set; }

        //public string PassengerImgUrl { get; set; }

        //public DateTime PassportExpireDate { get; set; }

    }
}
