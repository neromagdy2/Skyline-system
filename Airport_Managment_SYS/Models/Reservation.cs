namespace Airport_Managment_SYS.Models
{
    
    public class Reservation
    {
        public int TripId { get; set; }
        public Trip Trip { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
