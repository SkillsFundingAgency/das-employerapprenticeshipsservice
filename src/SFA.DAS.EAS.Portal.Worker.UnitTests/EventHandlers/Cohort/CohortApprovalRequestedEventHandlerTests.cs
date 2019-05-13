using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments;
using SFA.DAS.EAS.Portal.Worker.UnitTests.Builders;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Worker.UnitTests.EventHandlers.Cohort
{
    [TestFixture]
    public class CohortApprovalRequestedEventHandlerTests
    {
        public class TestContext
        {
            public CohortApprovalRequestedByProviderEventHandler Sut { get; private set; }
            public Mock<ICommandHandler<CohortApprovalRequestedCommand>> MockHandler { get; private set; }
            public Mock<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>> MockAdapter { get; private set; }
            public Mock<IMessageHandlerContext> MockMessageHandlerContext { get; private set; }

            public TestContext()
            {
                MockHandler = new Mock<ICommandHandler<CohortApprovalRequestedCommand>>();
                MockAdapter = new Mock<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>>();
                MockMessageHandlerContext = new Mock<IMessageHandlerContext>();

                Sut = new CohortApprovalRequestedByProviderEventHandler(MockHandler.Object, MockAdapter.Object);
            }
        }        

        public class Handle : CohortApprovalRequestedEventHandlerTests
        {
            [Test]
            public async Task WhenCalled_ThenTheAdapterIsCalledToConvertTheMessage()
            {
                // arrange
                var testContext = new TestContext();
                CohortApprovalRequestedByProvider @event = new CohortApprovalRequestedByProviderBuilder();

                // act
                await testContext.Sut.Handle(@event, testContext.MockMessageHandlerContext.Object);

                //assert
                testContext.MockAdapter.Verify(m => m.Convert(@event), Times.Once);
            }

            [Test]
            public async Task WhenCalled_ThenTheCommandHandlerIsCalled()
            {
                // arrange
                var testContext = new TestContext();
                CohortApprovalRequestedByProvider @event = new CohortApprovalRequestedByProviderBuilder();
                CohortApprovalRequestedCommand command = new CohortApprovalRequestedCommandBuilder();

                testContext.MockAdapter
                    .Setup(m => m.Convert(@event))
                    .Returns(command);

                // act
                await testContext.Sut.Handle(@event, testContext.MockMessageHandlerContext.Object);

                //assert
                testContext.MockHandler.Verify(m => m.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
