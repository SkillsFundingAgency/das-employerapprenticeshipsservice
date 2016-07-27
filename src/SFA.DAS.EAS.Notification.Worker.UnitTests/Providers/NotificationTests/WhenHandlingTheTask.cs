using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;

namespace SFA.DAS.EAS.Notification.Worker.UnitTests.Providers.NotificationTests
{
    
    public class WhenHandlingTheTask
    {
        private Worker.Providers.Notification _notification;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _notification = new Worker.Providers.Notification(_logger.Object);
        }

        [Test]
        public void ThenTheQueueIsRead()
        {

        }
    }
}
