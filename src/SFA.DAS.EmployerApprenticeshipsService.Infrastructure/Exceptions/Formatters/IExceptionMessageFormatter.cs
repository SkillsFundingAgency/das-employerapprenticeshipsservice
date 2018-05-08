using System;

namespace SFA.DAS.EAS.Infrastructure.Exceptions
{
    internal interface IExceptionMessageFormatter
    {
        string GetFormattedMessage(Exception exception);
    }
}
