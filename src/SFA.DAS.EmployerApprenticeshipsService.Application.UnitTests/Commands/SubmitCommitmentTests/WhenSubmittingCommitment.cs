using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SubmitCommitmentTests
{

    [TestFixture]
    public class WhenSubmittingCommitment
    {
        private SubmitCommitmentCommandHandler _handler;
        private Mock<ICommitmentsApi> _mockCommitmentApi;
        private Mock<ITasksApi> _mockTasksApi;
        private Mock<IEventsApi> _mockEventsApi;
        private SubmitCommitmentCommand _validCommand;

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand { EmployerAccountId = 12L, CommitmentId = 2L };

            _mockCommitmentApi = new Mock<ICommitmentsApi>();

            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new Commitment { ProviderId = 456L, EmployerAccountId = 12L });
            _mockTasksApi = new Mock<ITasksApi>();
            _mockEventsApi = new Mock<IEventsApi>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentApi.Object, _mockTasksApi.Object, _mockEventsApi.Object);
        }

        [Test]
        public async Task ThenTheCommitmentApiShouldBeCalled()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerCommitment(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CommitmentSubmission>()));
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
        public async Task ThenAnEventShouldBeCreated()
        {
            await _handler.Handle(_validCommand);

            _mockEventsApi.Verify(x => x.CreateAgreementEvent(It.IsAny<AgreementEvent>()));
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
