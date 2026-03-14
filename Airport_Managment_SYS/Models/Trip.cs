using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Models
{
    public class Trip
    {
        public int Id { get; set; }
        
        [Range(25.00, float.MaxValue, ErrorMessage = "Minimum trip price is 25.00 EGP to meet payment processing requirements.")]
        public float Price { get; set; }
        
        public DateTime DateTime { get; set; }

        [DateGreaterThan(nameof(DateTime))]
        public DateTime ArrivalDateTime { get; set; }
        public int AirplaneId{ get; set; }
        public int Airport_ToId { get; set; }

        [NotEqual(nameof(Airport_ToId),ErrorMessage = "The Distanation Airport and Deprture Airport can't be the same")]
        public int Airport_FromId { get; set; }

        public bool IsDeleted { get; set; }

        public List<TripSeat>? TripSeats { get; set; }

        public Airplane? Airplane{ get; set; }
        public Airport? Airport_To { get; set; }

        public Airport? Airport_From { get; set; }

    }
}
