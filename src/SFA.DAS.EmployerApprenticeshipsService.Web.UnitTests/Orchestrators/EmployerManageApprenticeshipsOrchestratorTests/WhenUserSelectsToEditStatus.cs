using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenUserSelectsToEditStatus
    {
        private EmployerManageApprenticeshipsOrchestrator _sut;
        private ApprenticeshipMapper _mockApprenticeshipMapper;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockDateTime;
        private Apprenticeship _testApprenticeship;

        [SetUp]
        public void SetUp()
        {
            _testApprenticeship = new Apprenticeship
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = new DateTime(1988, 4, 5),
                PaymentStatus = PaymentStatus.Active,
                StartDate = DateTime.UtcNow.AddMonths(1) // Default start date a month in the future.
            };

            _mockMediator = new Mock<IMediator>();
            _mockDateTime = new Mock<ICurrentDateTime>();
            _mockApprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), _mockDateTime.Object, _mockMediator.Object);

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = _testApprenticeship
                });

            _sut = new EmployerManageApprenticeshipsOrchestrator(_mockMediator.Object, Mock.Of<IHashingService>(), _mockApprenticeshipMapper, new CurrentDateTime(), Mock.Of<ILogger>());
        }

        [TestCase(PaymentStatus.Active)]
        [TestCase(PaymentStatus.Paused)]
        public async Task ThenShouldSkipSelectingChangeDateIfTrainingHasNotStarted(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [TestCase(PaymentStatus.Active)]
        [TestCase(PaymentStatus.Paused)]
        public async Task ThenShouldNotSkipSelectingChangeDateIfTrainingStarted(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1).Date;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipStep.Should().BeFalse();
        }

        [Test]
        public async Task ThenStartedTrainingAndImmediateChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, null, "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task ThenStartedTrainingAndSpecicDateSpecifiedShouldSetDateOfChangeToSpecifiedDate()
        {
            var specifiedDate = DateTime.UtcNow.AddMonths(-1).Date;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.SpecificDate, specifiedDate, "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(specifiedDate);
        }
    }
}
