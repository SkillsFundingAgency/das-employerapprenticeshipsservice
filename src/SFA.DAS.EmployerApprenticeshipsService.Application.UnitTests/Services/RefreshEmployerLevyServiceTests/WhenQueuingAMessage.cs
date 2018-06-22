using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Messages.Commands;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Services.RefreshEmployerLevyServiceTests
{
    public class WhenQueuingAMessage
    {
        private RefreshEmployerLevyService _refreshEmployerLevyService;
        private Mock<IEndpointInstance> _endpoint;

        [SetUp]
        public void Arrange()
        {
            _endpoint = new Mock<IEndpointInstance>();

            _refreshEmployerLevyService = new RefreshEmployerLevyService(_endpoint.Object);
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
            _endpoint.Verify(x => x.Send(It.Is<IImportAccountLevyDeclarationsCommand>(c => c.AccountId.Equals(expectedAccountId) && c.PayeRef.Equals(expectedPayeRef))));
        }
    }
}
