using System;

namespace SFA.DAS.EmployerAccounts.Messages
{
    public interface IUserMessage
    {
        Guid? UserRef { get; set; }
    }
}