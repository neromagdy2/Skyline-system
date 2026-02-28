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
}
