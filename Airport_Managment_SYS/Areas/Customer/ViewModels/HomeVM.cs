namespace Airport_Managment_SYS.Areas.Customer.ViewModels
{
    public class HomeVM
    {
        public SearchTripsVM SearchTripsVM { get; set; }
        public List<Trip> ?trips { get; set; }
    }
}
