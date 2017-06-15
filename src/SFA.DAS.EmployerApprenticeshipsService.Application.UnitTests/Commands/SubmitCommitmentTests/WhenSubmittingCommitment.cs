using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.SendNotification;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using FluentAssertions;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SubmitCommitmentTests
{
    [TestFixture]
    public sealed class WhenSubmittingCommitment
    {
        private SubmitCommitmentCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private SubmitCommitmentCommand _validCommand;
        private Mock<IMediator> _mockMediator;
        private Mock<IProviderEmailLookupService> _mockEmailLookup;
        private CommitmentView _repositoryCommitment;

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand { EmployerAccountId = 12L, CommitmentId = 2L, UserDisplayName = "Test User", UserEmailAddress = "test@test.com", UserId = "externalUserId"};
            _repositoryCommitment = new CommitmentView
            {
                ProviderId = 456L,
                EmployerAccountId = 12L,
                AgreementStatus = AgreementStatus.NotAgreed
            };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_repositoryCommitment);

            _mockMediator = new Mock<IMediator>();
            var config = new EmployerApprenticeshipsServiceConfiguration
                             {
                                 CommitmentNotification = new CommitmentNotificationConfiguration { SendEmail = true }
                             };
            _mockEmailLookup = new Mock<IProviderEmailLookupService>();
            _mockEmailLookup.Setup(m => m.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new List<string>());

            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentApi.Object, _mockMediator.Object, config, _mockEmailLookup.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenTheCommitmentApiShouldBeCalled()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerCommitment(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CommitmentSubmission>()));
        }

        [Test]
        public async Task NotCallGetProviderEmailQueryRequest()
        {
            _validCommand.LastAction = LastAction.None;
            await _handler.Handle(_validCommand);


            _mockEmailLookup.Verify(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task NotShouldCallSendNotificationCommand()
        {
            _validCommand.LastAction = LastAction.None;
            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Never);
        }

        [Test]
        public async Task CallGetProviderEmailQueryRequest()
        {
            _validCommand.LastAction = LastAction.Amend;
            await _handler.Handle(_validCommand);

            _mockEmailLookup.Verify(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForCohortReview()
        {
            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Amend;
            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x );

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("ProviderCommitmentNotification");
            arg.Email.Tokens["type"].Should().Be("review");
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForCohortFirstApproval()
        {
            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Approve;

            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("ProviderCommitmentNotification");
            arg.Email.Tokens["type"].Should().Be("approval");
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForCohortSecondApproval()
        {
            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Approve;
            _repositoryCommitment.AgreementStatus = AgreementStatus.ProviderAgreed;

            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("ProviderCohortApproved");
            arg.Email.Tokens["type"].Should().Be("approval");
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandOncePerEmailAddress()
        {
            _validCommand.LastAction = LastAction.Amend;
            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com", "test2@email.com" });

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Exactly(2));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.EmployerAccountId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException2()
        {
            _validCommand.EmployerAccountId = 2;

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }
    }
}

