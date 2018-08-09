using System;

namespace SFA.DAS.EmployerFinance.Messages
{
    public interface IUserMessage
    {
        Guid? UserRef { get; set; }
    }
}