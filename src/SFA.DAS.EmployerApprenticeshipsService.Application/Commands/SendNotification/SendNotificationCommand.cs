using System;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification
{
    public class SendNotificationCommand : IAsyncRequest
    {
        public long UserId { get; set; }
        public DateTime DateTime { get; set; }
        public bool ForceFormat { get; set; }
        public string TemplatedId { get; set; }
        public EmailContent Data { get; set; }
        public MessageFormat MessageFormat { get; set; }

    }
}