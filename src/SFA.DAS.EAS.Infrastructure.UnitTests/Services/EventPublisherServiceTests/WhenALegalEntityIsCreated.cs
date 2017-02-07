using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.EventPublisherServiceTests
{
    [TestFixture]
    public class WhenALegalEntityIsCreated : EventPublisherServiceTestsBase
    {
        [Test]
        public async Task ThenTheEventIsCreated()
        {
            var hashedAccountId = "ABC123";
            var legalEntityId = 9576;
            var expectedAccountResourceUri = $"api/accounts/{hashedAccountId}/legalentities/{legalEntityId}";

            await Publisher.PublishLegalEntityCreatedEvent(hashedAccountId, legalEntityId);

            Mediator.Verify(x => x.PublishAsync(It.Is<CreateAccountEventCommand>(e => e.ResourceUri == expectedAccountResourceUri && e.Event == "LegalEntityCreated")), Times.Once);
        }
    }
}
