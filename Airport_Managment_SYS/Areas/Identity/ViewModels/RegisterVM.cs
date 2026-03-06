using System.ComponentModel.DataAnnotations;
using Airport_Managment_SYS.Models;

namespace Airport_Managment_SYS.Areas.Identity.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "User name is required")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Please select nationality")]
        public int? NationalitiesId { get; set; }


        public IEnumerable<Nationalities>? Nationalities { get; set; }
    }
}