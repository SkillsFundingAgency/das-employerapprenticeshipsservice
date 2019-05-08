using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Commitments.Events;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.Commands.Cohort;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.Builders;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers.Cohort
{
    [TestFixture]
    public class CohortApprovalRequestedEventHandlerTests
    {
        private CohortApprovalRequestedByProviderEventHandler _sut;
        private Mock<ICommandHandler<CohortApprovalRequestedCommand>> _mockHandler;
        private Mock<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>> _mockAdapter;
        private Mock<IMessageHandlerContext> _mockMessageHandlerContext;        

        public CohortApprovalRequestedEventHandlerTests()
        {
            _mockHandler = new Mock<ICommandHandler<CohortApprovalRequestedCommand>>();
            _mockAdapter = new Mock<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>>();
            _mockMessageHandlerContext = new Mock<IMessageHandlerContext>();

            _sut = new CohortApprovalRequestedByProviderEventHandler(_mockHandler.Object, _mockAdapter.Object);
        }

        public class Handle: CohortApprovalRequestedEventHandlerTests
        {
            [Test]
            public async Task WhenCalled_ThenTheAdapterIsCalledToConvertTheMessage()
            {
                // arrange
                CohortApprovalRequestedByProvider @event = new ApprovalRequestedBuilder();

                // act
                await _sut.Handle(@event, _mockMessageHandlerContext.Object);

                //assert
                _mockAdapter.Verify(m => m.Convert(@event), Times.Once);
            }
            
            [Test]
            public async Task WhenCalled_ThenTheCommandHandlerIsCalled()
            {
                // arrange
                CohortApprovalRequestedByProvider @event = new ApprovalRequestedBuilder();
                CohortApprovalRequestedCommand command = new CreateCohortCommandBuilder();

                _mockAdapter
                    .Setup(m => m.Convert(@event))
                    .Returns(command);

                // act
                await _sut.Handle(@event, _mockMessageHandlerContext.Object);

                //assert
                _mockHandler.Verify(m => m.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
            }            
        }
    }
}
