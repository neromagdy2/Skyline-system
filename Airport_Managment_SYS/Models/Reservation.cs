namespace Airport_Managment_SYS.Models
{
    
    public class Reservation
    {
        public int TripId { get; set; }
        public Trip Trip { get; set; }

        public int NumSeats { get; set; }
        public List<Seat> Seats { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
