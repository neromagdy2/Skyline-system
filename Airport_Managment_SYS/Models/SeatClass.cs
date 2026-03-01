using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Models
{
    public class SeatClass
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        
        public double PriceMult { get; set; } = 1;

    }
}
