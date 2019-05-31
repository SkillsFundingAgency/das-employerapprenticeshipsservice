using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Testing;

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
        
        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldExecuteAddAccountProviderCommand()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyCommandExecutedWithUnchangedEvent());
        }
    }

    public class AddedAccountProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        AddedAccountProviderEvent, AddedAccountProviderEventHandler, ICommand<AddedAccountProviderEvent>>
    {
    }
}