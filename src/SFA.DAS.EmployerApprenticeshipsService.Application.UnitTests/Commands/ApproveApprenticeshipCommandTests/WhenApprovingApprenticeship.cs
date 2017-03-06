﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.ApproveApprenticeship;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.ApproveApprenticeshipCommandTests
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
            _validCommand = new ApproveApprenticeshipCommand
                { EmployerAccountId = 12L, CommitmentId = 2L, ApprenticeshipId = 4L, UserId = "externalUserId"};

            _mockCommitmentApi = new Mock<ICommitmentsApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new Commitment { ProviderId = 456L });
            _handler = new ApproveApprenticeshipCommandHandler(_mockCommitmentApi.Object);
        }

        [Test]
        public async Task ThenTheCommitmentApiShouldBeCalled()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerApprenticeship(
                It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.Is<ApprenticeshipSubmission>(y => y.PaymentStatus == PaymentStatus.Active)));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.ApprenticeshipId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnExceptionWhenUserIdMissing()
        {
            _validCommand.UserId = string.Empty;

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }

    }
}
