using Microsoft.AspNetCore.Mvc.Rendering;

namespace Airport_Managment_SYS.Areas.Customer.ViewModels
{
    public class SearchTripsVM
    {
        public int DepartureCity { get; set; }
        public int ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public IEnumerable<Trip> ?trips { get; set; }
        public IEnumerable<GovernerateState>? States { get; set; }
    }

    public class DetailsTripVM
    {
        public Trip Trip { get; set; } = null!;
        public int AvailableSeats { get; set; }
        public int SeatsToReserve { get; set; }
        public IEnumerable<SeatClass>? SeatClasses { get; set; }
        public Dictionary<int,int>? AvailableByClass { get; set; }
        public int SelectedSeatClassId { get; set; }
    }
}
