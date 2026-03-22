using System.Threading.Tasks;

namespace Airport_Managment_SYS.Utilities
{
    public interface ISkyStreamEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName);
    }
}
