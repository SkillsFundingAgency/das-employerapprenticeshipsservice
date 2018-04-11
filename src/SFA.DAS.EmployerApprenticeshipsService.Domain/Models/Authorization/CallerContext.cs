using System;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public class CallerContext : ICallerContext
    {
        public long? AccountId { get; set; }
        public Guid? UserExternalId { get; set; }
    }
}