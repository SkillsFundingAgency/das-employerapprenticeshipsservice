using System;
using System.Globalization;
using NServiceBus;

namespace SFA.DAS.EAS.Portal.Application.Services.MessageContext
{
    public class MessageContext : IMessageContext, IMessageContextInitialisation
    {
        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        
        private const string TimeSentFormat = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        public void Initialise(IMessageHandlerContext handlerContext)
        {
            Id = handlerContext.MessageId;
            CreatedDateTime = ToUtcDateTime(handlerContext.MessageHeaders["NServiceBus.TimeSent"]);
        }

        private static DateTime ToUtcDateTime(string wireFormattedString)
        {
            return DateTime
                .ParseExact(wireFormattedString, TimeSentFormat, CultureInfo.InvariantCulture)
                .ToUniversalTime();
        }
    }
}
