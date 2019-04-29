using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.Commands.CreateCohort;
using SFA.DAS.EmployerAccounts.Events.Cohort;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.Builders;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers.Cohort
{
    [TestFixture]
    public class CohortCreatedEventHandlerTests
    {
        private CohortCreatedEventHandler _sut;
        private Mock<ICommandHandler<CreateCohortCommand>> _mockHandler;
        private Mock<IAdapter<CohortCreated, CreateCohortCommand>> _mockAdapter;
        private Mock<IMessageHandlerContext> _mockMessageHandlerContext;        

        public CohortCreatedEventHandlerTests()
        {
            _mockHandler = new Mock<ICommandHandler<CreateCohortCommand>>();
            _mockAdapter = new Mock<IAdapter<CohortCreated, CreateCohortCommand>>();
            _mockMessageHandlerContext = new Mock<IMessageHandlerContext>();

            _sut = new CohortCreatedEventHandler(_mockHandler.Object, _mockAdapter.Object);
        }

        public class Handle: CohortCreatedEventHandlerTests
        {
            [Test]
            public async Task WhenCalled_ThenTheAdapterIsCalledToConvertTheMessage()
            {
                // arrange
                CohortCreated @event = new CohortCreatedBuilder();

                // act
                await _sut.Handle(@event, _mockMessageHandlerContext.Object);

                //assert
                _mockAdapter.Verify(m => m.Convert(@event), Times.Once);
            }

            [Test]
            public async Task WhenCalled_ThenTheCommandHandlerIscalledWithTheCreateCohortCommand()
            {
                // arrange
                CohortCreated @event = new CohortCreatedBuilder();
                CreateCohortCommand command = new CreateCohortCommandBuilder();

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
