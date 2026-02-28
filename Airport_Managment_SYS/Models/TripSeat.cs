using Microsoft.EntityFrameworkCore;

namespace Airport_Managment_SYS.Models
{
    [PrimaryKey(nameof(TripId),nameof(SeatId))]
    public class TripSeat
    {
        public int TripId { get; set; }
        public int SeatId { get; set; }
        public Trip Trip { get; set; }
        public Seat Seat { get; set; }

        public bool IsBooked { get; set; }
    }
}
