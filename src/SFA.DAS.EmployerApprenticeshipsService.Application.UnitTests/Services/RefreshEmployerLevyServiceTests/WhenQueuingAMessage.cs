using FluentAssertions;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Messages.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Services.RefreshEmployerLevyServiceTests
{
    public class WhenQueuingAMessage
    {
        private RefreshEmployerLevyService _refreshEmployerLevyService;
        private TestableEndpointInstance _endpoint;

        [SetUp]
        public void Arrange()
        {
            _endpoint = new TestableEndpointInstance();

            _refreshEmployerLevyService = new RefreshEmployerLevyService(_endpoint);
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
            _endpoint.SentMessages.Length.Should().Be(1);

            var message = _endpoint.SentMessages.Select(m => m.Message.As<IImportAccountLevyDeclarationsCommand>())
                .Single(m => m != null);

            message.AccountId.Should().Be(expectedAccountId);
            message.PayeRef.Should().Be(expectedPayeRef);
        }
    }
}
