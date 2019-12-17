using System;
using System.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.Time
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now => _now ?? DateTime.Now;

        private readonly DateTime? _now;

        public CurrentDateTime()
        {
            var setting = ConfigurationManager.AppSettings["CurrentTime"];

            if (DateTime.TryParse(setting, out var now))
            {
                _now = now;
            }
        }
    }
}