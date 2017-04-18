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

using SFA.DAS.EAS.Web.Validators;

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

            _sut = new EmployerManageApprenticeshipsOrchestrator(
                _mockMediator.Object, 
                Mock.Of<IHashingService>(), 
                _mockApprenticeshipMapper, 
                new ApprovedApprenticeshipViewModelValidator(), 
                new CurrentDateTime(), 
                Mock.Of<ILogger>());
        }

        [TestCase(PaymentStatus.Active)]
        [TestCase(PaymentStatus.Paused)]
        public async Task ThenShouldSkipSelectingChangeDateIfTrainingHasNotStarted(PaymentStatus paymentStatus)
        {
            _testApprenticeship.PaymentStatus = paymentStatus;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Stop, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfPausingLiveApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Pause, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfResumingApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Paused;

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Resume, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfPausingWaitingToStartApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Active;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Already started

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Pause, "user123");

            response.Data.SkipStep.Should().BeTrue();
        }

        [Test]
        public async Task ThenShouldSkipSelectingChangeDateIfResumingWaitingToStartApprenticeship()
        {
            _testApprenticeship.PaymentStatus = PaymentStatus.Paused;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Already started

            OrchestratorResponse<WhenToMakeChangeViewModel> response = await _sut.GetChangeStatusDateOfChangeViewModel("ABC123", "CDE321", ChangeStatusType.Resume, "user123");

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
        public async Task IfStoppingThenStartedTrainingAndImmediateChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, null, "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfStoppingThenStartedTrainingAndSpecicDateSpecifiedShouldSetDateOfChangeToSpecifiedDate()
        {
            var specifiedDate = DateTime.UtcNow.AddMonths(-1).Date;
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel("ABC123", "CDE321", ChangeStatusType.Stop, WhenToMakeChangeOptions.SpecificDate, specifiedDate, "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(specifiedDate);
        }

        [Test]
        public async Task IfPausingAndStartedTrainingThenChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel(
                "ABC123", 
                "CDE321", 
                ChangeStatusType.Pause, 
                WhenToMakeChangeOptions.Immediately, 
                null, 
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfPausingAndWaitingToStartTrainingThenChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(2); // Apprenticeship is waiting to start

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Pause,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfResumingAndStartedTrainingThenChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(-1); // Apprenticeship has already started

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Resume,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public async Task IfResumingAndWaitingToStartTrainingThenChangeSpecifiedShouldSetDateOfChangeToTodaysDate()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(2); // Apprenticeship is waiting to start

            OrchestratorResponse<ConfirmationStateChangeViewModel> response = await _sut.GetChangeStatusConfirmationViewModel(
                "ABC123",
                "CDE321",
                ChangeStatusType.Resume,
                WhenToMakeChangeOptions.Immediately,
                null,
                "user123");

            response.Data.ChangeStatusViewModel.DateOfChange.DateTime.Should().Be(DateTime.UtcNow.Date);
        }
    }
}
