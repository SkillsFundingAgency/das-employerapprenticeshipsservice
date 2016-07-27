using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public EmailService(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(EmailMessage message)
        {
            
            using (var client = new SmtpClient())
            {
                client.Port = GetPortNumber(_configuration.SmtpServer.Port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = _configuration.SmtpServer.ServerName;

                if (!string.IsNullOrEmpty(_configuration.SmtpServer.UserName) && !string.IsNullOrEmpty(_configuration.SmtpServer.Password))
                {
                    client.Credentials = new System.Net.NetworkCredential(_configuration.SmtpServer.UserName, _configuration.SmtpServer.Password);
                }

                var mail = new MailMessage(message.ReplyToAddress, message.RecipientsAddress)
                {
                    Subject = message.MessageType,
                    Body = JsonConvert.SerializeObject(message)
                };
                await client.SendMailAsync(mail);
            }
        }

        private int GetPortNumber(string candidate)
        {
            int port;

            return int.TryParse(candidate, out port) ? port : 25;
        }
    }
}
