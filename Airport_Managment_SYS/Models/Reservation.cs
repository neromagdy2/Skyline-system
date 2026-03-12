namespace Airport_Managment_SYS.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        public int TripId { get; set; }
        public Trip Trip { get; set; }

        public int NumSeats { get; set; } = 1;
        
        public int SeatClassId { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsPaid { get; set; } = false;

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        
        public List<ReservationSeat> ReservationSeats { get; set; } = new List<ReservationSeat>();
    }
}
