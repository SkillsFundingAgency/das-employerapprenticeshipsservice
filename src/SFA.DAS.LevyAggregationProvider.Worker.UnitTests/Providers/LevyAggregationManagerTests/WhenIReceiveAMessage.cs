using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateLevyAggregation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.LevyAggregationProvider.Worker.Providers;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyAggregationProvider.Worker.UnitTests.Providers.LevyAggregationManagerTests
{
    public class WhenIReceiveAMessage
    {
        private LevyAggregationManager _levyAggregationManager;
        private Mock<ILogger> _logger;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();
            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _mediator = new Mock<IMediator>();

            _levyAggregationManager = new LevyAggregationManager(_pollingMessageReceiver.Object, _mediator.Object,_logger.Object);
        }

        [Test]
        public async Task ThenTheMessageIsReadFromTheQueue()
        {
            //Act
            await _levyAggregationManager.Process();

            //Assert
            _pollingMessageReceiver.Verify(x=>x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>(), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var mockFileMessage = new Mock<Message<EmployerRefreshLevyQueueMessage>>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).ReturnsAsync(mockFileMessage.Object);

            //Act
            await _levyAggregationManager.Process();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetLevyDeclarationRequest>()), Times.Never);
            _mediator.Verify(x=>x.SendAsync(It.IsAny<CreateLevyAggregationCommand>()), Times.Never);
            mockFileMessage.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task Then
    }
}
