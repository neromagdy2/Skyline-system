using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class CreateGovernorateVM
    {
        [MaxLength(100), MinLength(2)]
        [Required]
        public string Name { get; set; }

            public int CountryId { get; set; }

            public IEnumerable<Country> ?Countries { get; set; }
        
    
}
}
