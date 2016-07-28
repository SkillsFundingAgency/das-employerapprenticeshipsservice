using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification
{
    public class NotificationMessage
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public DateTime DateTime { get; set; }
        public bool ForceFormat { get; set; }
        public string TemplatedId { get; set; }
        public string Data { get; set; }
        public MessageFormat MessageFormat { get; set; }
    }
}
