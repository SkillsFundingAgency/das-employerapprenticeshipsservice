using System.Threading.Tasks;
using NLog;

namespace SFA.DAS.EAS.Notification.Worker.Providers
{
    public class Notification : INotification
    {
        private readonly ILogger _logger;

        public Notification(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Handle()
        {
            _logger.Info("Service has started");
        }
    }
}
