using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.Areas.Admin.ViewModels
{
    public class UpdateChatbotQuestionVM
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Question { get; set; }
        public int? ParentId { get; set; }
        [MaxLength(100)]
        public string Answer { get; set; }
        public string Type { get; set; }
        public IEnumerable<ChatbotQuestion>? Questions { get; set; }
    }
}
