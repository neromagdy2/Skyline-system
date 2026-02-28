using System.ComponentModel.DataAnnotations;
using Airport_Managment_SYS.Models;

namespace Airport_Managment_SYS.Areas.Identity.ViewModels
{
    public class RegisterVM
    {
 
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]

        public string ConfirmPassword { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid nationality")]
        public int? NationalitiesId { get; set; }
        public string PhoneNumber { get; set; }

        public IEnumerable<Nationalities>? Nationalities { get; set; }
    }
}
