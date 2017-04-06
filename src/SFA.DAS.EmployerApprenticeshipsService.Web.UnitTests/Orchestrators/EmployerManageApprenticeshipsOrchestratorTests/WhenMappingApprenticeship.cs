using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using System;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetApprenticeshipUpdate;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenMappingApprenticeship
    {
        private EmployerManageApprenticeshipsOrchestrator _orchestrator;
        private ApprenticeshipMapper _apprenticeshipMapper;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockDateTime;
        private Mock<ICookieStorageService<ApprenticeshipViewModel>> _cookieStorageService;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _mockDateTime = new Mock<ICurrentDateTime>();

            _cookieStorageService = new Mock<ICookieStorageService<ApprenticeshipViewModel>>();

            _apprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), _mockDateTime.Object, _mockMediator.Object);

            _orchestrator = new EmployerManageApprenticeshipsOrchestrator(_mockMediator.Object, Mock.Of<IHashingService>(), _apprenticeshipMapper, Mock.Of<ILogger>(),_cookieStorageService.Object);
        }

        [TestCase(8, 5, arg3: 10)]
        [TestCase(8, 5, arg3: 9)]
        public async Task ShouldSetStatusTextForApprenticeshipNotStarted(int nowMonth, int nowDay, int startMonth)
        {
            _mockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, nowMonth, nowDay));
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                                  {
                                      Apprenticeship = 
                                        new Apprenticeship
                                        {
                                            PaymentStatus = PaymentStatus.Active,
                                            StartDate = new DateTime(1998, startMonth, 1)
                                        }
                                  });
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await _orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);

            result.Data.Status.Should().Be("Waiting to start");
        }

        [TestCase(8, 5, arg3: 7)]
        [TestCase(8, 1, arg3: 2)]
        [TestCase(8, 1, arg3: 8, Description = "Start date is the same month as now")]
        [TestCase(8, 5, arg3: 8)]
        public async Task ShouldSetStatusTextForApprenticeshipStarted(int nowMonth, int nowDay, int startMonth)
        {
            _mockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, nowMonth, nowDay));
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                                        new Apprenticeship
                                        {
                                            PaymentStatus = PaymentStatus.Active,
                                            StartDate = new DateTime(1998, startMonth, 1)
                                        }
                });

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            var result = await _orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);

            result.Data.Status.Should().Be("On programme");
        }

        [Test]
        public async Task ShouldSetRecordStatusTextForApprenticeshipWithUpdateWaitingForApproval()
        {
            _mockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, 12, 8));
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                                        new Apprenticeship
                                        {
                                            PaymentStatus = PaymentStatus.Active,
                                            StartDate = new DateTime(1998, 11, 1)
                                        }
                });

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse
                                  {
                                      ApprenticeshipUpdate = 
                                          new ApprenticeshipUpdate
                                          {
                                              ApprenticeshipId = 1L,
                                              Originator = Originator.Employer
                                          }
                                  });

            var result = await _orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);
            result.Data.PendingChanges.Should().Be(PendingChanges.WaitingForApproval);
        }

        [Test]
        public async Task ShouldSetRecordStatusTextForApprenticeshipWithUpdateReadyForReview()
        {
            _mockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, 12, 8));
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                        new Apprenticeship { PaymentStatus = PaymentStatus.Active, StartDate = new DateTime(1998, 11, 1) }
                });

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse
                {
                    ApprenticeshipUpdate =
                        new ApprenticeshipUpdate {ApprenticeshipId = 1L, Originator = Originator.Provider }
                });

            var result = await _orchestrator.GetApprenticeship("hashedAccountId", "hashedApprenticeshipId", "UserId");
            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);
            result.Data.PendingChanges.Should().Be(PendingChanges.ReadyForApproval);
        }

        [Test]
        public async Task ThenTheApprenticeshipViewModelIsMappedToTheUpdateApprenticeshipViewModel()
        {
            //Arrange
            var updateApprenticeshipViewModel = new UpdateApprenticeshipViewModel
            {
                LastName = "Tester",
                FirstName = "Test",
                NINumber = "134AD",
                ULN = "trgf4567",
                DateOfBirth = new DateTimeViewModel {Day = 10, Month = 3, Year = 2000},
                Cost = "12345",
                EmployerRef = "123ERD",
                ProviderRef = "654TRF",
                StartDate = new DateTimeViewModel {Day = 11, Month = 4, Year = 2018},
                EndDate = new DateTimeViewModel {Day = 12, Month = 5, Year = 2019}

            };

            //Act
            var actual = await _apprenticeshipMapper.MapToUpdateApprenticeshipViewModel(updateApprenticeshipViewModel);

            //Assert
            actual.ShouldBeEquivalentTo(updateApprenticeshipViewModel);
        }
    }
}
