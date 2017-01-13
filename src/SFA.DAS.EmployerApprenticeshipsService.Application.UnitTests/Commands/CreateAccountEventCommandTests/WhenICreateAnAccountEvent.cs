using System;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateAccountEventCommandTests
{
    [TestFixture]
    public class WhenICreateAnAccountEvent
    {
        private Mock<IEventsApi> _eventApi;
        private Mock<ILogger> _logger;
        private CreateAccountEventCommandHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _eventApi = new Mock<IEventsApi>();
            _logger = new Mock<ILogger>();
            _handler = new CreateAccountEventCommandHandler(_eventApi.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheEventIsCreated()
        {
            var command = new CreateAccountEventCommand { Event = "Created", HashedAccountId = "ABC123" };

            await _handler.Handle(command);

            _eventApi.Verify(x => x.CreateAccountEvent(It.Is<AccountEvent>( e => e.Event == command.Event && e.EmployerAccountId == command.HashedAccountId)), Times.Once);
        }

        [Test]
        public async Task AndTheEventCreationFails()
        {
            var expectedException = new Exception();
            var command = new CreateAccountEventCommand { Event = "Created", HashedAccountId = "ABC123" };

            _eventApi.Setup(x => x.CreateAccountEvent(It.Is<AccountEvent>(e => e.Event == command.Event && e.EmployerAccountId == command.HashedAccountId))).Throws(expectedException);

            await _handler.Handle(command);

            _logger.Verify(x => x.Error(expectedException));
        }
    }
}
