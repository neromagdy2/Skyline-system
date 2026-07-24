using Airport_Managment_SYS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class EditUserVM
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public string? SelectedRole { get; set; }

        public IEnumerable<SelectListItem>? AvailableRoles { get; set; }
    }
}
