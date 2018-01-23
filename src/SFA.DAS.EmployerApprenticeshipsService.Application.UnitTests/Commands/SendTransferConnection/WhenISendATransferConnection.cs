using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnection;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SendTransferConnection
{
    public class WhenISendATransferConnection
    {
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;
        private const long TransferConnectionId = 333333;
        private const string FirstName = "John";
        private const string LastName = "Doe";

        private SendTransferConnectionCommandHandler _handler;
        private SendTransferConnectionCommand _command;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly Mock<ITransferConnectionRepository> _transferConnectionRepository = new Mock<ITransferConnectionRepository>();
        private Mock<IMessagePublisher> _messagePublisher;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId, UserRef = ExternalUserId, FirstName = FirstName, LastName = LastName };
        private TransferConnection _transferConnection;
        private TransferConnection _savedTransferConnection;
        private TransferConnectionInvitationSentMessage _message;

        [SetUp]
        public void SetUp()
        {
            _transferConnection = new TransferConnection
            {
                Id = TransferConnectionId,
                SenderAccountId = SenderAccountId,
                ReceiverAccountId = ReceiverAccountId,
                Status = TransferConnectionStatus.Created
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();

            _transferConnectionRepository.Setup(r => r.GetCreatedTransferConnection(TransferConnectionId)).ReturnsAsync(_transferConnection);
            _transferConnectionRepository.Setup(r => r.Send(It.IsAny<TransferConnection>())).Returns(Task.CompletedTask).Callback<TransferConnection>(c => _savedTransferConnection = c);
            _membershipRepository.Setup(r => r.GetCaller(SenderAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _messagePublisher.Setup(p => p.PublishAsync(It.IsAny<object>())).Returns(Task.CompletedTask).Callback<object>(m => _message = m as TransferConnectionInvitationSentMessage);

            _handler = new SendTransferConnectionCommandHandler(_currentUser, _membershipRepository.Object, _transferConnectionRepository.Object, _messagePublisher.Object);
            _command = new SendTransferConnectionCommand { TransferConnectionId = TransferConnectionId };
        }

        [Test]
        public async Task ThenShouldGetTransferConnection()
        {
            await _handler.Handle(_command);

            _transferConnectionRepository.Verify(r => r.GetCreatedTransferConnection(TransferConnectionId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_command);

            _membershipRepository.Verify(r => r.GetCaller(SenderAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenShouldUpdateTransferConnection()
        {
            await _handler.Handle(_command);

            _transferConnectionRepository.Verify(r => r.Send(_transferConnection), Times.Once);
            
            Assert.That(_savedTransferConnection, Is.SameAs(_transferConnection));
            Assert.That(_savedTransferConnection.Status, Is.EqualTo(TransferConnectionStatus.Sent));
        }

        [Test]
        public async Task ThenShouldPublishTransferConnectionInvitationSentMessage()
        {
            await _handler.Handle(_command);

            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<TransferConnectionInvitationSentMessage>()), Times.Once);

            Assert.That(_message, Is.Not.Null);
            Assert.That(_message.TransferConnectionId, Is.EqualTo(TransferConnectionId));
            Assert.That(_message.SenderAccountId, Is.EqualTo(SenderAccountId));
            Assert.That(_message.AccountId, Is.EqualTo(ReceiverAccountId));
            Assert.That(_message.CreatorName, Is.EqualTo($"{FirstName} {LastName}"));
            Assert.That(_message.CreatorUserRef, Is.EqualTo(ExternalUserId));
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(SenderAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }
    }
}