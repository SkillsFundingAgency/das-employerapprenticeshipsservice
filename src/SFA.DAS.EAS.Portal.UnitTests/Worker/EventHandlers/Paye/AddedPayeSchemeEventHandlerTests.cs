using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Paye;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.PayeScheme;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Paye
{
    [Parallelizable]
    [TestFixture]
    class AddedPayeSchemeEventHandlerTests
    {
        private AddedPayeSchemeEventHandler _sut;
        private Mock<ICommandHandler<PayeSchemeAddedCommand>> _commandHandlerMock;
        private Mock<IMessageHandlerContext> _messageHandlerContextMock;
        private Mock<IMessageContext> _messageContextMock;

        [SetUp]
        public void SetUp()
        {
            _messageContextMock = new Mock<IMessageContext>();
            _commandHandlerMock = new Mock<ICommandHandler<PayeSchemeAddedCommand>>();
            _messageHandlerContextMock = new Mock<IMessageHandlerContext>();
            _messageHandlerContextMock
                   .Setup(m => m.MessageHeaders)
                   .Returns(
                   new Dictionary<string, string>
                   {
                        { "NServiceBus.TimeSent", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture) }
                   });
            _sut = new AddedPayeSchemeEventHandler(_commandHandlerMock.Object, _messageContextMock.Object);
        }

        [Test]
        [Category("UnitTest")]
        public async Task ShouldAddPayeSchemeToAccount()
        {
            // Arrange
            var userRef = Guid.NewGuid();
            var payeEvent = new AddedPayeSchemeEvent
            {
                AccountId = 1,
                Created = new DateTime(2019, 5, 31),
                PayeRef = "PayePayePaye",
                UserName = "Bob",
                UserRef = userRef
            };

            PayeSchemeAddedCommand resultCommand = null;

            _commandHandlerMock
                .Setup(mock => mock.Handle(It.IsAny<PayeSchemeAddedCommand>(), It.IsAny<CancellationToken>()))
                .Callback((PayeSchemeAddedCommand command, CancellationToken ct) => 
                {
                    resultCommand = command;
                })
                .Returns(Task.CompletedTask);

            // Act 
            await _sut.Handle(payeEvent, _messageHandlerContextMock.Object);

            // Assert
            resultCommand.AccountId.Should().Be(1);
            resultCommand.Created.Should().Be(new DateTime(2019, 5, 31));
            resultCommand.PayeRef.Should().Be("PayePayePaye");
            resultCommand.UserName.Should().Be("Bob");
            resultCommand.UserRef.Should().Be(userRef);
        }
    }
}
