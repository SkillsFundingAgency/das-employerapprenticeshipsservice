using System.Linq;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenMappingApprenticeshipAlertMessages
    {
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockDateTime;
        private ApprenticeshipMapper _apprenticeshipMapper;

        private EmployerManageApprenticeshipsOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _mockDateTime = new Mock<ICurrentDateTime>();

            _apprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), _mockDateTime.Object, _mockMediator.Object);
        }

        [Test]
        public void ShouldNotCreateAlertsForEmptyApprenticeship()
        {
            var apprenticeship = new Apprenticeship();
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(0);
        }

        [Test]
        public void EmployerCreatesChangeOfCircs()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Employer };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes pending");
        }

        [Test]
        public void ProviderCreatesChangeOfCircs()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Provider };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void UnTriagedDataLock07()
        {
            var apprenticeship = new Apprenticeship { DataLockPrice = true};
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(0);
        }

        [Test]
        public void TriagedDataLock07()
        {
            var apprenticeship = new Apprenticeship { DataLockPriceTriaged = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void UnTriagedDataLockCourse()
        {
            var apprenticeship = new Apprenticeship { DataLockCourse = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(0);
        }

        [Test]
        public void TriagedDataLockCourse()
        {
            var apprenticeship = new Apprenticeship { DataLockCourseTriaged = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes requested");
        }

        [Test]
        public void CoCAndUntriagedPriceDataLock()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Provider ,  DataLockPrice = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void CoCAndUntriagedCourseDataLock()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Provider, DataLockCourse = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void DataLockPriceOneTriagedAndOneNotTriaged()
        {
            var apprenticeship = new Apprenticeship { DataLockPrice = true, DataLockPriceTriaged = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void DataLockCourseOneTriagedAndOneNotTriaged()
        {
            var apprenticeship = new Apprenticeship { DataLockPrice = true, DataLockCourseTriaged = true };
            var viewModel = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes requested");
        }


    }
}
