using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.ViewModels
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
        public int NationalityId { get; set; }
        public string PhoneNumber { get; set; }

        public IEnumerable<Nationalities>? Nationalities { get; set; }
    }
}
