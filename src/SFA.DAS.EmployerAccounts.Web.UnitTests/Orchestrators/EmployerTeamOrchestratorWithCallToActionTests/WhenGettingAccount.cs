using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;
using SFA.DAS.EmployerAccounts.TestCommon;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorWithCallToActionTests
{
    public class WhenGettingAccount
    {
        private readonly string _hashedAccountId = Guid.NewGuid().ToString();
        private AccountDashboardViewModel _accountDashboardViewModel;

        private readonly string _userId = Guid.NewGuid().ToString();

        private EmployerTeamOrchestratorWithCallToAction _sut;
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<EmployerTeamOrchestrator> _employerTeamOrchestrator = new();

        private Mock<ICurrentDateTime> _mockCurrentDateTime;
        private Mock<IAccountApiClient> _mockAccountApiClient;
        private Mock<IMapper> _mockMapper;
        private Mock<ICookieStorageService<AccountContext>> _mockAccountContext;
        private Mock<ILogger<EmployerTeamOrchestratorWithCallToAction>> _mockLogger;

        [SetUp]
        public void Arrange()
        {

            _mockAccountContext = new Mock<ICookieStorageService<AccountContext>>();

            _accountDashboardViewModel = new AccountDashboardViewModel
            {
                HashedAccountId = _hashedAccountId
            };

            _employerTeamOrchestrator
                .Setup(m => m.GetAccount(_hashedAccountId, _userId))
                .ReturnsAsync(new OrchestratorResponse<AccountDashboardViewModel>
                {
                    Data = _accountDashboardViewModel,
                    Status = HttpStatusCode.OK
                });

            _mediator.Setup(x => x.Send(It.IsAny<GetVacanciesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetVacanciesResponse
                {
                    Vacancies = new List<Vacancy>()
                });

            _mediator.Setup(m => m.Send(It.Is<GetReservationsRequest>(q => q.HashedAccountId == _hashedAccountId), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new GetReservationsResponse
                   {
                       Reservations = new List<Reservation>
                       {
                             new Reservation
                             {
                                 AccountId = 123
                             }
                       }
                   });

            _mediator.Setup(m => m.Send(It.Is<GetApprenticeshipsRequest>(q => q.HashedAccountId == _hashedAccountId), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new GetApprenticeshipsResponse
                 {
                     Apprenticeships = new List<Apprenticeship>
                    {
                        new Apprenticeship { FirstName = "FirstName" }
                    }
                 });

            var cohort = new Cohort()
            {
                Id = 1,
                CohortStatus = EmployerAccounts.Models.CommitmentsV2.CohortStatus.WithTrainingProvider,
                NumberOfDraftApprentices = 1,
                Apprenticeships = new List<Apprenticeship>
                        {
                            new Apprenticeship()
                            {
                                Id = 2,
                                FirstName = "FirstName",
                                LastName = "LastName",
                                CourseStartDate = new DateTime(2020,5,1),
                                CourseEndDate = new DateTime(2022,1,1),
                                CourseName = "CourseName"
                            }
                        }
            };
            _mediator.Setup(x => x.Send(It.IsAny<GetSingleCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetSingleCohortResponse
                    {
                        Cohort = cohort

                    });

            _mockCurrentDateTime = new Mock<ICurrentDateTime>();

            _mockAccountApiClient = new Mock<IAccountApiClient>();

            _mockAccountApiClient.Setup(c => c.GetAccount(_hashedAccountId)).ReturnsAsync(new AccountDetailViewModel
            { ApprenticeshipEmployerType = "Levy" });

            _mockMapper = new Mock<IMapper>();

            _mockLogger = new Mock<ILogger<EmployerTeamOrchestratorWithCallToAction>>();

            _sut = new EmployerTeamOrchestratorWithCallToAction(
                _employerTeamOrchestrator.Object,
                _mediator.Object,
                _mockCurrentDateTime.Object,
                _mockAccountApiClient.Object,
                _mockMapper.Object,
                _mockAccountContext.Object,
                _mockLogger.Object,
                Mock.Of<EmployerAccountsConfiguration>());
        }

        [Test]
        public async Task ThenAccountDataIsRetrievedFromTheTeamOrchestrator()
        {
            // Act
            await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _employerTeamOrchestrator.Verify(m => m.GetAccount(_hashedAccountId, _userId), Times.Once);
        }

        [Test]
        public async Task ThenExpectedAccountDataIsReturnedFromTheTeamOrchestrator()
        {
            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.Should().Be(_accountDashboardViewModel);
        }

        [Test]
        public async Task ThenCallToActionDataIsRetrievedWhenAccountContextCookieIsNotSet()
        {
            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().NotBeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsRetrievedWhenAccountContextIsNonLevy()
        {
            // arrange
            _mockAccountContext
                .Setup(m => m.Get(EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName))
                .Returns(new AccountContext { HashedAccountId = _hashedAccountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().NotBeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenAccountContextIsLevy()
        {
            // arrange
            _mockAccountContext
                .Setup(m => m.Get(EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName))
                .Returns(new AccountContext { HashedAccountId = _hashedAccountId, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenTheAccountContextIsSavedWhenAccountContextIsLevy()
        {
            // arrange
            _accountDashboardViewModel.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _mockAccountContext.Verify(m => m.Delete(EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName), Times.Once);
            _mockAccountContext.Verify(m => m.Create(It.Is<AccountContext>(a =>
                (a.HashedAccountId == _hashedAccountId) && (a.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)),
                EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName,
                It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public async Task ThenTheAccountContextIsSavedWhenAccountContextIsNonLevy()
        {
            // arrange
            _accountDashboardViewModel.ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy;

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _mockAccountContext.Verify(m => m.Delete(EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName), Times.Once);
            _mockAccountContext.Verify(m => m.Create(It.Is<AccountContext>(a =>
                (a.HashedAccountId == _hashedAccountId) && (a.ApprenticeshipEmployerType == ApprenticeshipEmployerType.NonLevy)),
                EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName,
                It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public async Task ThenCallToActionDataIsRetrievedWhenAccountContextAccountHasChanged()
        {
            // arrange
            _mockAccountContext
                .Setup(m => m.Get(EmployerTeamOrchestratorWithCallToAction.AccountContextCookieName))
                .Returns(new AccountContext { HashedAccountId = Guid.NewGuid().ToString(), ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().NotBeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenVacancyServiceHasFailed()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetVacanciesRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetVacanciesResponse
               {
                   HasFailed = true
               });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenVacancyServiceHasException()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetVacanciesRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenErrorIsLoggedWhenVacancyServiceHasException()
        {
            //Arrange
            var exception = new Exception();
            _mediator.Setup(x => x.Send(It.IsAny<GetVacanciesRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(exception);

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _mockLogger.VerifyLogging($"An error occurred whilst trying to retrieve account CallToAction: {_hashedAccountId}", LogLevel.Error, Times.Once());
        }

        [Test]
        public async Task ThenShouldReturnTheVacancies()
        {
            //Arrange            
            var vacancy = new Vacancy { Title = Guid.NewGuid().ToString() };
            var vacancies = new List<Vacancy> { vacancy };

            var expectedtitle = Guid.NewGuid().ToString();
            var expectedvacancy = new VacancyViewModel { Title = expectedtitle };
            var expectedVacancies = new List<VacancyViewModel> { expectedvacancy };

            _mediator.Setup(x => x.Send(It.IsAny<GetVacanciesRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetVacanciesResponse
               {
                   Vacancies = vacancies
               });

            _mockMapper.Setup(m => m.Map<IEnumerable<Vacancy>, IEnumerable<VacancyViewModel>>(vacancies))
                .Returns(expectedVacancies);

            // Act
            var actual = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.AreEqual(1, actual.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount);
            Assert.AreEqual(expectedvacancy.Title, actual.Data.CallToActionViewModel.VacanciesViewModel.Vacancies.First().Title);
            _mockMapper.Verify(m => m.Map<IEnumerable<Vacancy>, IEnumerable<VacancyViewModel>>(vacancies), Times.Once);
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenReservationsServiceHasFailed()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetReservationsRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetReservationsResponse
               {
                   HasFailed = true
               });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenReservationsServiceHasException()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetReservationsRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenErrorIsLoggedWhenReservationsServiceHasException()
        {
            //Arrange
            var exception = new Exception();
            _mediator.Setup(x => x.Send(It.IsAny<GetReservationsRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(exception);

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _mockLogger.VerifyLogging($"An error occurred whilst trying to retrieve account CallToAction: {_hashedAccountId}", LogLevel.Error, Times.Once());
        }

        [Test]
        public async Task ThenShouldGetReservationsCount()
        {
            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            Assert.AreEqual(1, result.Data.CallToActionViewModel.ReservationsCount);
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenApprenticeshipsServiceHasFailed()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetApprenticeshipsResponse
               {
                   HasFailed = true
               });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenApprenticeshipsServiceHasException()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenErrorIsLoggedWhenApprenticeshipsServiceHasException()
        {
            //Arrange
            var exception = new Exception();
            _mediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(exception);

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _mockLogger.VerifyLogging($"An error occurred whilst trying to retrieve account CallToAction: {_hashedAccountId}", LogLevel.Error, Times.Once());
        }

        [Test]
        public async Task ThenShouldGetApprenticeshipResponse()
        {
            //Arrange
            var apprenticeships = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName" } };
            _mediator.Setup(m => m.Send(It.Is<GetApprenticeshipsRequest>(q => q.HashedAccountId == _hashedAccountId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetApprenticeshipsResponse { Apprenticeships = apprenticeships });
            var expectedApprenticeship = new List<ApprenticeshipViewModel>() { new ApprenticeshipViewModel { ApprenticeshipFullName = "FullName" } };
            _mockMapper.Setup(m => m.Map<IEnumerable<Apprenticeship>, IEnumerable<ApprenticeshipViewModel>>(apprenticeships)).Returns(expectedApprenticeship);

            //Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            Assert.IsTrue(result.Data.CallToActionViewModel.Apprenticeships.Count().Equals(1));
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenCohortsServiceHasFailed()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetSingleCohortRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetSingleCohortResponse
               {
                   HasFailed = true
               });

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenCallToActionDataIsNotRetrievedWhenCohortsServiceHasException()
        {
            //Arrange

            _mediator.Setup(x => x.Send(It.IsAny<GetSingleCohortRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            result.Data.CallToActionViewModel.Should().BeNull();
        }

        [Test]
        public async Task ThenErrorIsLoggedWhenCohortsServiceHasException()
        {
            //Arrange
            var exception = new Exception();
            _mediator.Setup(x => x.Send(It.IsAny<GetSingleCohortRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(exception);

            // Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert
            _mockLogger.VerifyLogging($"An error occurred whilst trying to retrieve account CallToAction: {_hashedAccountId}", LogLevel.Error, Times.Once());
        }

        [Test]
        public async Task ThenShouldGetCohortResponse()
        {
            //Arrange 
            var cohort = new Cohort() { Id = 1, NumberOfDraftApprentices = 1, Apprenticeships = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName" } } };
            _mediator.Setup(x => x.Send(It.IsAny<GetSingleCohortRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetSingleCohortResponse { Cohort = cohort });
            var expectedCohort = new CohortViewModel()
            {
                NumberOfDraftApprentices = 1,
                Apprenticeships = new List<ApprenticeshipViewModel> { new ApprenticeshipViewModel { ApprenticeshipFullName = "FullName" } }
            };
            _mockMapper.Setup(m => m.Map<Cohort, CohortViewModel>(cohort)).Returns(expectedCohort);

            //Act
            var result = await _sut.GetAccount(_hashedAccountId, _userId);

            //Assert                    
            Assert.AreEqual(1, result.Data.CallToActionViewModel.CohortsCount);
        }
    }
}
