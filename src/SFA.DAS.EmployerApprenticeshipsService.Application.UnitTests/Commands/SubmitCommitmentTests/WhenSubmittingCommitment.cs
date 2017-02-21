using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Moq;

using NLog;

using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.SendNotification;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SubmitCommitmentTests
{

    [TestFixture]
    public class WhenSubmittingCommitment
    {
        private SubmitCommitmentCommandHandler _handler;
        private Mock<ICommitmentsApi> _mockCommitmentApi;
        private Mock<ITasksApi> _mockTasksApi;
        private SubmitCommitmentCommand _validCommand;
        private Mock<IMediator> _mockMediator;
        private Mock<IProviderEmailLookupService> _mockEmailLookup;

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand { EmployerAccountId = 12L, CommitmentId = 2L, UserDisplayName = "Test User", UserEmailAddress = "test@test.com" };

            _mockCommitmentApi = new Mock<ICommitmentsApi>();

            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new Commitment { ProviderId = 456L, EmployerAccountId = 12L });
            _mockTasksApi = new Mock<ITasksApi>();

            _mockMediator = new Mock<IMediator>();
            var config = new EmployerApprenticeshipsServiceConfiguration
                             {
                                 EmailTemplates = new List<EmailTemplateConfigurationItem>
                                                      {
                                                          new EmailTemplateConfigurationItem
                                                              {
                                                                  TemplateType = EmailTemplateType.CommitmentNotification,
                                                                  Key = "this-is-a-key"
                                                              }
                                                      },
                                 CommitmentNotification = new CommitmentNotificationConfiguration { SendEmail = true }
                             };
            _mockEmailLookup = new Mock<IProviderEmailLookupService>();
            _mockEmailLookup.Setup(m => m.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new List<string>());
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentApi.Object, _mockTasksApi.Object, _mockMediator.Object, config, _mockEmailLookup.Object, Mock.Of<ILogger>());
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
        public async Task ShouldCallSendNotificationCommand()
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

        [Test]
        public async Task ShouldNotCallTasksApi()
        {
            _validCommand.CreateTask = false;
            await _handler.Handle(_validCommand);

            _mockTasksApi.Verify(m => m.CreateTask("", It.IsAny<Tasks.Api.Types.Task>()), Times.Never);
        }

        [Test]
        public async Task ShouldCallTasksApi()
        {
            _validCommand.CreateTask = true;
            await _handler.Handle(_validCommand);

            _mockTasksApi.Verify(m => m.CreateTask(It.IsAny<string>(), It.IsAny<Tasks.Api.Types.Task>()), Times.Once);
        }
    }
}

