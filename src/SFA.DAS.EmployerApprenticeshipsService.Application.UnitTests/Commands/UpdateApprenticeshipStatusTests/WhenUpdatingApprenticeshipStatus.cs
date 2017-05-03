﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EAS.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EAS.Domain.Models.Apprenticeship;
using System;
using MediatR;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdateApprenticeshipStatusTests
{
    [TestFixture]
    public sealed class WhenUpdatingApprenticeshipStatus
    {
        private UpdateApprenticeshipStatusCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockCurrentDateTime;
        private IValidator<UpdateApprenticeshipStatusCommand> _validator = new UpdateApprenticeshipStatusCommandValidator();
        private UpdateApprenticeshipStatusCommand _validCommand;

        [SetUp]
        public void Setup()
        {
            _validCommand = new UpdateApprenticeshipStatusCommand
            {
                EmployerAccountId = 12L,
                ApprenticeshipId = 4L,
                UserId = "externalUserId",
                ChangeType = ChangeStatusType.Stop,
                DateOfChange = DateTime.UtcNow.Date,
                UserEmailAddress = "test@email.com",
                UserDisplayName = "Bob"
            };

            var apprenticeshipFromApi = new Apprenticeship { StartDate = DateTime.UtcNow.AddMonths(-2).Date };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new CommitmentView { ProviderId = 456L });
            _mockMediator = new Mock<IMediator>();

            var apprenticeshipGetResponse = new GetApprenticeshipQueryResponse { Apprenticeship = apprenticeshipFromApi };
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>())).ReturnsAsync(apprenticeshipGetResponse);
            _mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockCurrentDateTime.SetupGet(x => x.Now).Returns(DateTime.UtcNow);

            _handler = new UpdateApprenticeshipStatusCommandHandler(_mockCommitmentApi.Object, _mockMediator.Object, _mockCurrentDateTime.Object, _validator);
        }

        [TestCase(ChangeStatusType.Stop, PaymentStatus.Withdrawn)]
        [TestCase(ChangeStatusType.Pause, PaymentStatus.Paused)]
        [TestCase(ChangeStatusType.Resume, PaymentStatus.Active)]
        public async Task ThenTheCommitmentApiShouldBeCalledWithCorrectStatus(ChangeStatusType type, PaymentStatus expectedStatus)
        {
            _validCommand.ChangeType = type;
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerApprenticeship(
                _validCommand.EmployerAccountId, _validCommand.ApprenticeshipId,
                It.Is<ApprenticeshipSubmission>(
                    y => y.PaymentStatus == expectedStatus && y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName && y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress)));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.ApprenticeshipId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }
    }
}
