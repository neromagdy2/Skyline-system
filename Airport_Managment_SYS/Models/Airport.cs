namespace Airport_Managment_SYS.Models
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int GovernerateStateId { get; set; }
        public GovernerateState GovernerateState { get; set; }

    }
}
