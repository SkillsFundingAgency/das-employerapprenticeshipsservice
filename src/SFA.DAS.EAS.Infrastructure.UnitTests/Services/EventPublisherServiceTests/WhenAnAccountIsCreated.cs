using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.EventPublisherServiceTests
{
    [TestFixture]
    public class WhenAnAccountIsCreated : EventPublisherServiceTestsBase
    {
        [Test]
        public async Task ThenTheEventIsCreated()
        {
            var hashedAccountId = "ABC123";
            var expectedAccountResourceUri = "api/accounts/" + hashedAccountId;

            await Publisher.PublishAccountCreatedEvent(hashedAccountId);

            Mediator.Verify(x => x.PublishAsync(It.Is<CreateAccountEventCommand>(e => e.ResourceUri == expectedAccountResourceUri && e.Event == "AccountCreated")), Times.Once);
        }
    }
}
