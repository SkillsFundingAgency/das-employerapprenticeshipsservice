using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Startup;

namespace SFA.DAS.EmployerAccounts.Startup
{
    public class DefaultStartup : IStartup
    {
        private readonly IEnumerable<IStartup> _startups;

        public DefaultStartup(IEnumerable<IStartup> startups)
        {
            _startups = startups;
        }

        public Task StartAsync()
        {
            return Task.WhenAll(_startups.Select(t => t.StartAsync()));
        }

        public Task StopAsync()
        {
            return Task.WhenAll(_startups.Reverse().Select(t => t.StopAsync()));
        }
    }
}
