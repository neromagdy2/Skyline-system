namespace Airport_Managment_SYS.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public DateTime DateTime { get; set; }
        public int Airport_ToId { get; set; }
        public int Airport_FromId { get; set; }

        public Airport Airport_To { get; set; }
        public Airport Airport_From { get; set; }

    }
}
