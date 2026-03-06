using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Models
{
    public class Country
    {
        public int Id { get; set; }

        [MaxLength(100), MinLength(2)]
        [Required]
        public string Name { get; set; } = null!;
        // International mobile dialing code, e.g. "+20"
        public string? MobileCode { get; set; }

        // Navigation - list of governorates/cities in this country
        public ICollection<GovernerateState>? Governerates { get; set; }
    }
}
