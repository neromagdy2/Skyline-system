namespace Airport_Managment_SYS.Models
{
    public class ReservationSeat
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public int SeatId { get; set; }
        public Seat Seat { get; set; }
    }
}
