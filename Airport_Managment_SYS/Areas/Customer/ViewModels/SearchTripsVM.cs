using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Customer.ViewModels
{
    public class SearchTripsVM
    {
        public int DepartureCity { get; set; }
        public int ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }

        public float MaxPrice { get; set; }
        public List<int >?SeatClassIds { get; set; }
        public IEnumerable<Trip> ?trips { get; set; }
        public IEnumerable<GovernerateState>? States { get; set; }
        public IEnumerable<SeatClass>? seatClasses { get; set; }
        
        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
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
