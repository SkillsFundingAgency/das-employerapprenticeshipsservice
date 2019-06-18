using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Testing;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.ProviderRelationships
{
    [TestFixture, Parallelizable]
    public class AddedAccountProviderEventHandlerTests : FluentTest<AddedAccountProviderEventHandlerFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }

        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldPassTheEventToTheDomainHandler()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyDomainHandlerCalled());
        }
    }

    public class AddedAccountProviderEventHandlerFixture : EventHandlerTestsFixture<AddedAccountProviderEvent, AddedAccountProviderEventHandler>
    {
        public Mock<IEventHandler<AddedAccountProviderEvent>> HandlerMock { get; set; }

        public AddedAccountProviderEventHandlerFixture()
        {
            HandlerMock = new Mock<IEventHandler<AddedAccountProviderEvent>>();

            Handler = new AddedAccountProviderEventHandler(
                HandlerMock.Object,
                MessageContextInitialisationMock.Object);
        }

        public void VerifyDomainHandlerCalled()
        {
            HandlerMock.Verify(s => s.Handle(Message, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
