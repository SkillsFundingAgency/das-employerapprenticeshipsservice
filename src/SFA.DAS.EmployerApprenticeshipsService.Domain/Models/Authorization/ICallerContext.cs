using System;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface ICallerContext
    {
        long? AccountId { get; }
        Guid? UserExternalId { get; }
    }
}