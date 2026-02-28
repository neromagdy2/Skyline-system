using Microsoft.AspNetCore.Identity;

namespace Airport_Managment_SYS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? NationalitiesId { get; set; }
        public Nationalities? National { get; set; }
    }
}
