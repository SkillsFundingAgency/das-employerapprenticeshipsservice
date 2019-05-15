using System;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public class MessageContext : IMessageContext
    {
        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
