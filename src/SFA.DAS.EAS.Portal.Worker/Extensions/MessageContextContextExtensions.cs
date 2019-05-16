using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;
using System;
using System.Globalization;

namespace SFA.DAS.EAS.Portal.Worker.Extensions
{
    public static class MessageContextContextExtensions
    {
        const string TimeSentFormat = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        public static void Initialise(this IMessageContext context, IMessageHandlerContext handlerContext)
        {
            context.Id = handlerContext.MessageId;
            context.CreatedDateTime = ToUtcDateTime(handlerContext.MessageHeaders["NServiceBus.TimeSent"]);
        }

        private static DateTime ToUtcDateTime(string wireFormattedString)
        {
            return DateTime
                .ParseExact(wireFormattedString, TimeSentFormat, CultureInfo.InvariantCulture)
                .ToUniversalTime();
        }
    }
}
