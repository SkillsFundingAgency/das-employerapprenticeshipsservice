using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments
{
    [TestFixture]
    [Parallelizable]
    public class CohortApprovalRequestedByProviderEventHandlerTests : FluentTest<CohortApprovalRequestedByProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
        
        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldExecuteCohortApprovalRequestedCommand()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyCommandExecutedWithUnchangedEvent());
        }
    }

    public class CohortApprovalRequestedByProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler, IPortalCommand<CohortApprovalRequestedByProvider>>
    {
    }
}
