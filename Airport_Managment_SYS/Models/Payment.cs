namespace Airport_Managment_SYS.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public float Total {  get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
