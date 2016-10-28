using System;

namespace SFA.DAS.EAS.Domain.Models.Notification
{
    public class NotificationMessage
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime DateTime { get; set; }
        public bool ForceFormat { get; set; }
        public string TemplatedId { get; set; }
        public string Data { get; set; }
        public MessageFormat MessageFormat { get; set; }
    }
}
