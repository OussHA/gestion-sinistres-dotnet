using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace ISH_APP.Services
{
    public class EmailSender
    {
        private readonly string _apiKey;

        public EmailSender(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("a.elaroud@ishabitat.com", "Intermédiaire Services Habitat");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: body);
            var response = await client.SendEmailAsync(msg);
        }
    }

}
