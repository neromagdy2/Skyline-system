using System.ComponentModel.DataAnnotations.Schema;

namespace Airport_Managment_SYS.Models
{
    public class PassengerTicketData
    {
        public int Id { get; set; }

        public string PassengerName { get; set; }

        public string PassengerNationality { get; set; }

        public string PassengerPassportNumber { get; set; }

        public string PassengerImgUrl { get; set; }

        public DateTime PassportExpireDate { get; set; }

   
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
