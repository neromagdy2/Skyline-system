namespace Airport_Managment_SYS.Models
{
    public class ChatbotQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int ?ChatbotQuestionId { get; set; }
        public string type { get; set; }
        public ChatbotQuestion ChatbotQuestionNav { get; set; }
    }
}
