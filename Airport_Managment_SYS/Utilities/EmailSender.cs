using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace MoviesApp.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("neromagdy2@gmail.com", "quvg lxdn sdfp nurw ")
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
    }
}
