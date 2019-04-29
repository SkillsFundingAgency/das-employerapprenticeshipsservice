using SFA.DAS.EmployerFinance.Interfaces;
using System;
using Microsoft.Azure;

namespace SFA.DAS.EmployerFinance.Time
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now => _now ?? DateTime.Now;

        private readonly DateTime? _now;

        public CurrentDateTime()
        {
            var setting = CloudConfigurationManager.GetSetting("CurrentTime");
            
            if (DateTime.TryParse(setting, out var now))
            {
                _now = now;
            }
        }
    }
}
