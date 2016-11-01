using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;
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

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand { EmployerAccountId = 12L, CommitmentId = 2L };

            _mockCommitmentApi = new Mock<ICommitmentsApi>();

            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new Commitment { ProviderId = 456L, EmployerAccountId = 12L });
            _mockTasksApi = new Mock<ITasksApi>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentApi.Object, _mockTasksApi.Object);
        }

        [Test]
        public async Task ThenTheCommitmentApiShouldBeCalled()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerCommitment(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CommitmentStatus>()));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.EmployerAccountId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }

        [Test]
        public async Task ThenATaskShouldBeCreated()
        {
            await _handler.Handle(_validCommand);

            _mockTasksApi.Verify(x => x.CreateTask(It.IsAny<string>(), It.IsAny<Tasks.Api.Types.Task>()));
        }
    }
}
