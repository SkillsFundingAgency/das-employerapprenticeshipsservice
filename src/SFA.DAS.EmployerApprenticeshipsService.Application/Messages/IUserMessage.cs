using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public interface IUserMessage
    {
        Guid? UserRef { get; set; }
    }
}