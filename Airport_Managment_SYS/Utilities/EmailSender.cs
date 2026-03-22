using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace Airport_Managment_SYS.Utilities
{
    public class EmailSender : ISkyStreamEmailSender, IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("neromagdy2@gmail.com", "eboo fmqb tgym tqjo ")
            };

            return client.SendMailAsync(
                new MailMessage(from: "neromagdy2@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                {
                    IsBodyHtml = true
                });
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("neromagdy2@gmail.com", "eboo fmqb tgym tqjo ")
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("neromagdy2@gmail.com", "SKYSTREAM"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                // Add PDF attachment
                if (attachment != null && attachment.Length > 0)
                {
                    var attachmentStream = new MemoryStream(attachment);
                    mailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentName, "application/pdf"));
                }

                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                // Log error here
                Console.WriteLine($"Failed to send email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
