using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ApproveApprenticeshipCommandTests
{
    [TestFixture]
    public sealed class WhenApprovingApprenticeship
    {
        private ApproveApprenticeshipCommandHandler _handler;
        private Mock<ICommitmentsApi> _mockCommitmentApi;
        private ApproveApprenticeshipCommand _validCommand;

        [SetUp]
        public void Setup()
        {
            _validCommand = new ApproveApprenticeshipCommand { EmployerAccountId = 12L, CommitmentId = 2L, ApprenticeshipId = 4L };

            _mockCommitmentApi = new Mock<ICommitmentsApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new Commitment { ProviderId = 456L });
            _handler = new ApproveApprenticeshipCommandHandler(_mockCommitmentApi.Object);
        }

        [Test]
        public async Task ThenTheCommitmentApiShouldBeCalled()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.Is<ApprenticeshipStatus>(y => y == ApprenticeshipStatus.Approved)));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.ApprenticeshipId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }
    }
}
