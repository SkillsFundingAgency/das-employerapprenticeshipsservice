using System;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface ICallerContext
    {
        string AccountHashedId { get; }
        long? AccountId { get; }
        Guid? UserRef { get; }
    }
}