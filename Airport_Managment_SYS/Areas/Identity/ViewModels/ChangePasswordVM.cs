using System.ComponentModel.DataAnnotations;

namespace MoviesApp.ViewModels
{
 
  public class ChangePasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }

}
