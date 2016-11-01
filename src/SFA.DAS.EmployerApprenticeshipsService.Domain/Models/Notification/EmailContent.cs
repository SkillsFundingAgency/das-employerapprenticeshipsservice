using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Notification
{
    public class EmailContent
    {
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}