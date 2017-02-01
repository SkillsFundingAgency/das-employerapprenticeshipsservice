using System.Threading.Tasks;
using System.Web;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.EventPublisherServiceTests
{
    [TestFixture]
    public class WhenAPayeSchemeIsRemoved : EventPublisherServiceTestsBase
    {
        [Test]
        public async Task ThenTheEventIsCreated()
        {
            var hashedAccountId = "ABC123";
            var payeSchemeRef = "ABC/123";
            var expectedAccountResourceUri = $"api/accounts/{hashedAccountId}/payeschemes/{HttpUtility.UrlEncode(payeSchemeRef)}";

            await Publisher.PublishPayeSchemeRemovedEvent(hashedAccountId, payeSchemeRef);

            Mediator.Verify(x => x.PublishAsync(It.Is<CreateAccountEventCommand>(e => e.ResourceUri == expectedAccountResourceUri && e.Event == "PayeSchemeRemoved")), Times.Once);
        }
    }
}
