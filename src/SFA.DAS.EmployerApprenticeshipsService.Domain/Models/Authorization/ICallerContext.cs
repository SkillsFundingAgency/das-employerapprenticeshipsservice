using System;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface ICallerContext
    {
        long? AccountId { get; }
        string ActionName { get; }
        string ControllerName { get; }
        Guid? UserExternalId { get; }
    }
}