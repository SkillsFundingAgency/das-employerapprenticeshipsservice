using System;
using SFA.DAS.EAS.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class SystemDateService : ISystemDateService
    {
        public DateTime Current => DateTime.Now;
    }
}