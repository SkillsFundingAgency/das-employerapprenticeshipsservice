using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using ApiProvider = SFA.DAS.Apprenticeships.Api.Types.Providers.Provider;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.ProviderRelationships
{
    [TestFixture]
    [Parallelizable]
    public class AddedAccountProviderEventHandlerTests : FluentTest<AddedAccountProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
    }

    public class AddedAccountProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        AddedAccountProviderEvent, AddedAccountProviderEventHandler>
    {
        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
        public ApiProvider Provider { get; set; }

        public AddedAccountProviderEventHandlerTestsFixture() 
            : base(false)
        {
            Fixture = new Fixture();
            Provider = Fixture.Create<ApiProvider>();

            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Message.ProviderUkprn)).ReturnsAsync(Provider);

            Handler = new AddedAccountProviderEventHandler(
                AccountDocumentService.Object,
                MessageContextInitialisation.Object,
                Logger.Object,
                ProviderApiClient.Object);
        }
    }
}