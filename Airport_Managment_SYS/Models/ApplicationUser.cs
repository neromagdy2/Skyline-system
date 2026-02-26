using Microsoft.AspNetCore.Identity;

namespace Airport_Managment_SYS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nationality { get; set; } = null!;
    }
}
