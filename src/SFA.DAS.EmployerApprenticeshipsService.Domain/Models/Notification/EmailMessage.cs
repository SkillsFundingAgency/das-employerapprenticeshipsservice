using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Notification
{
    public class EmailMessage
    {
        public long UserId { get; set; }
        public string MessageType { get; set; }
        public string TemplateId { get; set; }
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}