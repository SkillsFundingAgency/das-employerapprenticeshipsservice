using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.RefreshEmployerLevyServiceTests
{
    public class WhenQueuingAMessage
    {
        private RefreshEmployerLevyService _refreshEmployerLevyService;
        private Mock<IMessagePublisher> _messagePublisher;

        [SetUp]
        public void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();

            _refreshEmployerLevyService = new RefreshEmployerLevyService(_messagePublisher.Object);
        }

        [Test]
        public async Task ThenTheMessageIsAddedToTheQueueWithThePassedInParameters()
        {
            //Arrange
            var expectedAccountId = 123123;
            var expectedPayeRef = "123RFV";

            //Act
            await _refreshEmployerLevyService.QueueRefreshLevyMessage(expectedAccountId, expectedPayeRef);

            //Assert
            _messagePublisher.Verify(x=>x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(c=>c.AccountId.Equals(expectedAccountId) && c.PayeRef.Equals(expectedPayeRef))));
        }
    }
}
