
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    public class WhenGettingAccount
    {
        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        private const string UserId = "USER1";

        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;        
        private AccountStats _accountStats;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IMapper> _mapper;
        private List<AccountTask> _tasks;
        private AccountTask _testTask;

        private GetApprenticeshipsResponse ApprenticeshipsResponse;
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public GetCohortsResponse GetCohortsResponses { get; set; }
        public GetCohortsResponse GetCohortsResponseMoreThanOneDraftApprenticeship { get; set; }
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse { get; set; }

        [SetUp]
        public void Arrange()
        {
            _accountStats = new AccountStats
            {
                AccountId = 10,
                OrganisationCount = 3,
                PayeSchemeCount = 4,
                TeamMemberCount = 8
            };

            _testTask = new AccountTask
            {
                Type = "Test",
                ItemsDueCount = 2
            };

            _tasks = new List<AccountTask>
            {
                _testTask
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == HashedAccountId)))
                .ReturnsAsync(new GetEmployerAccountByHashedIdResponse
                {
                    Account = new Account
                    {
                        HashedId = HashedAccountId,
                        Id = AccountId,
                        Name = "Account 1"
                    }
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTasksQuery>()))
                .ReturnsAsync(new GetAccountTasksResponse
                {
                    Tasks = _tasks
                });

            _mediator.Setup(m => m.SendAsync(It.Is<GetUserAccountRoleQuery>(q => q.ExternalUserId == UserId)))
                     .ReturnsAsync(new GetUserAccountRoleResponse
                     {
                         UserRole = Role.Owner
                     });

            _mediator.Setup(m => m.SendAsync(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == HashedAccountId)))
                     .ReturnsAsync(new GetAccountEmployerAgreementsResponse
                     {
                         EmployerAgreements = new List<EmployerAgreementStatusDto>
                         {
                             new EmployerAgreementStatusDto
                             {
                                 Pending = new PendingEmployerAgreementDetailsDto { Id = 123 }
                             },
                             new EmployerAgreementStatusDto
                             {
                                 Pending = new PendingEmployerAgreementDetailsDto { Id = 124 }
                             },
                             new EmployerAgreementStatusDto
                             {
                                 Pending = new PendingEmployerAgreementDetailsDto { Id = 125 }
                             },
                             new EmployerAgreementStatusDto
                             {
                                 Pending = new PendingEmployerAgreementDetailsDto { Id = 126 }
                             },
                             new EmployerAgreementStatusDto
                             {
                                 Signed = new SignedEmployerAgreementDetailsDto { Id = 111 }
                             },
                             new EmployerAgreementStatusDto
                             {
                                 Signed = new SignedEmployerAgreementDetailsDto { Id = 112 }
                             },
                             new EmployerAgreementStatusDto
                             {
                                 Signed = new SignedEmployerAgreementDetailsDto { Id = 113 }
                             }
                         }
                     });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTeamMemberQuery>()))
                     .ReturnsAsync(new GetTeamMemberResponse{User = new MembershipView{FirstName = "Bob"}});

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountStatsQuery>()))
                     .ReturnsAsync(new GetAccountStatsResponse {Stats = _accountStats});


            _mediator.Setup(m => m.SendAsync(It.Is<GetReservationsRequest>(q => q.HashedAccountId == HashedAccountId)))
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
            
            _currentDateTime = new Mock<ICurrentDateTime>();

            GetCohortsResponse = CreateGetCohortsResponseForWithTrainingProviderStaus();
            GetCohortsResponses = CreateGetCohortsResponses();
            GetCohortsResponseMoreThanOneDraftApprenticeship = CreateGetCohortsResponseMoreThanOneDraftApprenticeships();
            DraftApprenticeshipsResponse = GetDraftApprenticeshipsResponse();

            _accountApiClient = new Mock<IAccountApiClient>();

            _accountApiClient.Setup(c => c.GetAccount(HashedAccountId)).ReturnsAsync(new AccountDetailViewModel
                {ApprenticeshipEmployerType = "Levy"});
           
            _mapper = new Mock<IMapper>();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, _currentDateTime.Object, _accountApiClient.Object,  _mapper.Object, Mock.Of<IAuthorizationService>());
        }
        
        [Test]
        public async Task ThenShouldGetAccountStats()
        {
            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.AreEqual(_accountStats.OrganisationCount, actual.Data.OrganisationCount);
            Assert.AreEqual(_accountStats.PayeSchemeCount, actual.Data.PayeSchemeCount);
            Assert.AreEqual(_accountStats.TeamMemberCount, actual.Data.TeamMemberCount);
        }

        [Test]
        public async Task ThenShouldReturnTasks()
        {
            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            _mediator.Verify(m => m.SendAsync(It.IsAny<GetAccountTasksQuery>()), Times.Once);
            Assert.IsNotNull(actual.Data);
            Assert.Contains(_testTask, actual.Data.Tasks.ToArray());
        }

        [Test]
        public async Task ThenIShouldNotReturnTasksWithZeroItems()
        {
            //Arrange
            _testTask.ItemsDueCount = 0;

            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.IsEmpty(actual.Data.Tasks);
        }

        [Test]
        public async Task ThenShouldReturnNoTasksIfANullIsReturnedFromTaskQuery()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTasksQuery>()))
                .ReturnsAsync(() => null);

            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.IsEmpty(actual.Data.Tasks);
        }

        [Test]
        public async Task ThenShouldReturnAccountsTasks()
        {
            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(_tasks, actual.Data.Tasks);
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountTasksQuery>(r => r.AccountId.Equals(AccountId))),Times.Once);
        }

        [Test]
        public async Task ThenShouldSetAccountHashId()
        {
            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(HashedAccountId, actual.Data.HashedAccountId);
        }

        [TestCase("2017-10-19", true, Description = "Banner visible")]
        [TestCase("2017-10-19 11:59:59", true, Description = "Banner visible until midnight")]
        [TestCase("2017-10-20 00:00:00", false, Description = "Banner hidden after midnight")]
        public async Task ThenDisplayOfAcademicYearBannerIsDetermined(DateTime now, bool expectShowBanner)
        {
            //Arrange
            _currentDateTime.Setup(x => x.Now).Returns(now);

            //Act
            var model = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(expectShowBanner, model.Data.ShowAcademicYearBanner);
        }

        [Test]
        public async Task ThenAgreementsAreRetrievedCorrectly()
        {
            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(3, actual.Data.SignedAgreementCount);
            Assert.AreEqual(4, actual.Data.RequiresAgreementSigning);
        }

        [TestCase(Common.Domain.Types.ApprenticeshipEmployerType.Levy, "Levy")]
        [TestCase(Common.Domain.Types.ApprenticeshipEmployerType.NonLevy, "NonLevy")]
        public async Task ThenShouldReturnCorrectApprenticeshipEmployerTypeFromAccountApi(Common.Domain.Types.ApprenticeshipEmployerType expectedApprenticeshipEmployerType, string apiApprenticeshipEmployerType)
        {
            //Arrange
            _accountApiClient
                .Setup(c => c.GetAccount(HashedAccountId))
                .ReturnsAsync(new AccountDetailViewModel
                { ApprenticeshipEmployerType = apiApprenticeshipEmployerType });

            //Act
            var model = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(expectedApprenticeshipEmployerType, model.Data.ApprenticeshipEmployerType);
        }
        
        [Test]
        public async Task ThenReturnAccountSummary()
        {
            var model = await _orchestrator.GetAccountSummary(HashedAccountId, UserId);
            Assert.IsNotNull(model.Data);
        }

        [Test]
        public async Task ThenShouldGetReservationsCount()
        {
            // Act
            var result = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(1, result.Data.CallToActionViewModel.ReservationsCount);
        }

        [Test]
        public async Task ThenReturnApprenticeshipResponse()
        {
            //Arrange
            ApprenticeshipsResponse = CreateApprenticeshipResponse();
            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetApprenticeship.GetApprenticeshipRequest>()))
            .ReturnsAsync(new Queries.GetApprenticeship.GetApprenticeshipResponse
            {
                ApprenticeshipDetailsResponse = ApprenticeshipsResponse
            });          

            //Act
            var result = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.CallToActionViewModel.ApprenticeshipsCount);
            Assert.AreEqual(0, result.Data.CallToActionViewModel.CohortsCount);
            Assert.IsFalse(result.Data.CallToActionViewModel.HasSingleDraftApprenticeship);
        }

        [Test]
        public async Task ThenShouldGetDraftApprenticeshipResponse()
        {
            //Arrange           
            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetCohorts.GetCohortsRequest>()))
           .ReturnsAsync(new Queries.GetCohorts.GetCohortsResponse
           {
               CohortsResponse = GetCohortsResponse,
               HashedCohortReference = "4_Encoded",
               SingleCohort = GetCohortsResponse.Cohorts.First()
           });

            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipRequest>()))
           .ReturnsAsync(new Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipResponse
           {
               DraftApprenticeshipsResponse = DraftApprenticeshipsResponse,
               HashedDraftApprenticeshipId = "1_Encoded"
           });

            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.IsTrue(actual.Data.CallToActionViewModel.ApprenticeshipsCount.Equals(0));
            Assert.IsTrue(actual.Data.CallToActionViewModel.CohortsCount.Equals(1));
            Assert.IsTrue(actual.Data.CallToActionViewModel.CohortStatus.Equals(CohortStatus.WithTrainingProvider));
            Assert.IsTrue(actual.Data.CallToActionViewModel.HasSingleDraftApprenticeship);
            Assert.AreEqual("1_Encoded", actual.Data.CallToActionViewModel.HashedDraftApprenticeshipId);
            Assert.AreEqual("4_Encoded", actual.Data.CallToActionViewModel.HashedCohortReference);
            Assert.AreEqual(expected.CourseName, actual.Data.CallToActionViewModel.CourseName);
            Assert.AreEqual(expected.StartDate, actual.Data.CallToActionViewModel.CourseStartDate);            
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenDraftApprenticeshipIsNullIfCohortsResponseIsNull()
        {
            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert            
            Assert.IsTrue(actual.Data.CallToActionViewModel.CohortsCount.Equals(0));
        }


        [Test]
        public async Task ThenGetDraftResponseIfCohortCountIsOne()
        {
            //Arrange           
            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetCohorts.GetCohortsRequest>()))
            .ReturnsAsync(new Queries.GetCohorts.GetCohortsResponse
            {
                CohortsResponse = GetCohortsResponse,
                HashedCohortReference = "4_Encoded",
                SingleCohort = GetCohortsResponse.Cohorts.First()
            });

            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipRequest>()))
           .ReturnsAsync(new Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipResponse
           {
               DraftApprenticeshipsResponse = DraftApprenticeshipsResponse,
               HashedDraftApprenticeshipId = "1_Encoded"
           });

            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.IsTrue(actual.Data.CallToActionViewModel.CohortsCount.Equals(1));
        }

        [Test]
        public async Task ThenDoNotGetDraftApprenticeshipsResponseIfCohortCountIsGreaterThanOne()
        {
            //Arrange           
            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetCohorts.GetCohortsRequest>()))
           .ReturnsAsync(new Queries.GetCohorts.GetCohortsResponse
           {
               CohortsResponse = GetCohortsResponses,
               HashedCohortReference = "4_Encoded",
               SingleCohort = GetCohortsResponse.Cohorts.First()
           });

            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipRequest>()))
           .ReturnsAsync(new Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipResponse
           {
               DraftApprenticeshipsResponse = DraftApprenticeshipsResponse,
               HashedDraftApprenticeshipId = "1_Encoded"
           });

            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert            
            Assert.AreEqual(actual.Data.CallToActionViewModel.CohortsCount, 0);
        }

        [Test]
        public async Task ThenNumberOfDraftApprenticeshipeGreaterThanOneDoNotGetDraftApprenticeshipsResponse()
        {
            //Arrange           
            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetCohorts.GetCohortsRequest>()))
           .ReturnsAsync(new Queries.GetCohorts.GetCohortsResponse
           {
               CohortsResponse = GetCohortsResponseMoreThanOneDraftApprenticeship,
               HashedCohortReference = "4_Encoded",
               SingleCohort = GetCohortsResponseMoreThanOneDraftApprenticeship.Cohorts.First()
           });

            _mediator.Setup(x => x.SendAsync(It.IsAny<Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipRequest>()))
           .ReturnsAsync(new Queries.GetSingleDraftApprenticeship.GetSingleDraftApprenticeshipResponse
           {
               DraftApprenticeshipsResponse = DraftApprenticeshipsResponse,
               HashedDraftApprenticeshipId = "1_Encoded"
           });

            //Act
            var result = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.AreEqual(result.Data.CallToActionViewModel.CohortsCount, 1);
            Assert.IsFalse(result.Data.CallToActionViewModel.HasSingleDraftApprenticeship);
            Assert.AreEqual("4_Encoded", result.Data.CallToActionViewModel.HashedCohortReference);
            Assert.AreEqual(GetCohortsResponseMoreThanOneDraftApprenticeship.Cohorts.First().NumberOfDraftApprentices, result.Data.CallToActionViewModel.NumberOfDraftApprentices);
        }


        private GetApprenticeshipsResponse CreateApprenticeshipResponse()
        {
            IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> apprenticeships = new List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()
            {
                new GetApprenticeshipsResponse.ApprenticeshipDetailsResponse
                {
                    Id = 1,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Uln = "Uln",
                    EmployerName = "EmployerName",
                    CourseName = "CourseName",
                    StartDate = new DateTime(2020, 5, 1),
                    EndDate = new DateTime(2022, 5, 1),
                    ApprenticeshipStatus = SFA.DAS.CommitmentsV2.Types.ApprenticeshipStatus.Live
                }
            };

            return new GetApprenticeshipsResponse() { Apprenticeships = apprenticeships, TotalApprenticeships = 1, TotalApprenticeshipsFound = 1, TotalApprenticeshipsWithAlertsFound = 0 };
        }


        private GetCohortsResponse CreateGetCohortsResponseForWithTrainingProviderStaus()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 1,
                    ProviderId = 4,
                    ProviderName = "Provider4",
                    NumberOfDraftApprentices = 1,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }

        private GetCohortsResponse CreateGetCohortsResponseMoreThanOneDraftApprenticeships()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 1,
                    ProviderId = 4,
                    ProviderName = "Provider4",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }


        private GetCohortsResponse CreateGetCohortsResponses()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 1,
                    ProviderId = 4,
                    ProviderName = "Provider4",
                    NumberOfDraftApprentices = 1,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
                new CohortSummary
                {
                    CohortId = 5,
                    AccountId = 1,
                    ProviderId = 5,
                    ProviderName = "Provider5",
                    NumberOfDraftApprentices = 2,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                }
            };

            return new GetCohortsResponse(cohorts);
        }

        private GetDraftApprenticeshipsResponse GetDraftApprenticeshipsResponse()
        {
            IReadOnlyCollection<DraftApprenticeshipDto> draftApprenticeships = new List<DraftApprenticeshipDto>()
            {
                new DraftApprenticeshipDto
                {
                    Id = 1,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = new DateTime(2000, 1 ,1 ),
                    Cost = 100,
                    StartDate = new DateTime(2020, 5, 1),
                    EndDate = new DateTime(2022, 5, 1),
                    CourseCode = "CourseCode",
                    CourseName = "CourseName"
                }
            };

            var draftApprenticeshipsResponse = new GetDraftApprenticeshipsResponse() { DraftApprenticeships = draftApprenticeships };
            return draftApprenticeshipsResponse;
        }
    }
}
