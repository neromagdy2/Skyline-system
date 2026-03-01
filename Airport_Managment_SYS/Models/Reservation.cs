namespace Airport_Managment_SYS.Models
{
    
    public class Reservation
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public Trip Trip { get; set; }

        public int NumSeats { get; set; }
        // SeatClass chosen by the user
        public int SeatClassId { get; set; }

        // Total price for this reservation (computed client-side and validated server-side)
        public decimal TotalPrice { get; set; }

        // Has the payment been completed?
        public bool IsPaid { get; set; } = false;

        public List<Seat> Seats { get; set; } = new List<Seat>();

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
