using System.ComponentModel.DataAnnotations;

namespace MoviesApp.ViewModels
{
    public class RegisterVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]

        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }


    }
}
