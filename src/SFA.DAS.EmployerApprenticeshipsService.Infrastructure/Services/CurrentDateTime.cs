using System;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now { get; }

        public CurrentDateTime()
        {
            Now = DateTime.UtcNow;
        }

        public CurrentDateTime(DateTime time)
        {
            Now = time;
        }
    }
}
