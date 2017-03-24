using System;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
