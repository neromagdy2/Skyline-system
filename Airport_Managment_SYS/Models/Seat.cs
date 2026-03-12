using System.Numerics;

namespace Airport_Managment_SYS.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int SeatNumber { get; set; }
        public bool Available { get; set; }
        public int seatClassId { get; set; }
        public int AirplaneId { get; set; }

        public List<TripSeat>? TripSeats { get; set; }
        public List<ReservationSeat>? ReservationSeats { get; set; }
        public SeatClass ?SeatClass { get; set; }
        public Airplane ?Airplane { get; set; }
    }
}
