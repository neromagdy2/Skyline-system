namespace Airport_Managment_SYS.Models
{
    public class GovernerateState
    {
        public int Id { get; set; }
    public string Name { get; set; } = null!;

        public int CountryId { get; set; }
        public Country ?Country { get; set; } 
    }
}
