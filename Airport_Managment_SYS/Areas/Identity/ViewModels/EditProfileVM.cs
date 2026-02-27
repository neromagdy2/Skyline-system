using System.ComponentModel.DataAnnotations;

namespace MoviesApp.ViewModels
{
    public class EditProfileVM
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Nationality")]
        public int NationalitiesId{ get; set; }
        public Nationalities Nationalities { get; set; }
    }
}

