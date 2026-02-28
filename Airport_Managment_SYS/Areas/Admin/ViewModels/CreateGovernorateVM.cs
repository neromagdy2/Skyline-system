namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateGovernorateVM
    {
      
            public string Name { get; set; }

            public int CountryId { get; set; }

            public IEnumerable<Country> ?Countries { get; set; }
        
    
}
}
