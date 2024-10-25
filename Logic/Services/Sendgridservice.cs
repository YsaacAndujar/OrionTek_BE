using Logic.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;


namespace Logic.Services
{
    public class SendgridService : IEmailSender
    {
        private readonly SendGridClient sendGridClient;
        private readonly EmailAddress from;
        public SendgridService(IConfiguration _configuration, SendGridClient _sendGridClient)
        {
            sendGridClient = _sendGridClient;
            from = new EmailAddress(_configuration["SendGrid:fromEmail"], "OrionTek");
        }
        public async Task SendEmail(string toEmail, string toName, string subject, string htmlContent, string plainTextContent)
        {
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await sendGridClient.SendEmailAsync(msg);
        }
    }
}
