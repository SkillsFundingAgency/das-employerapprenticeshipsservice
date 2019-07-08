//TODO: reinstate
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.EAS.Portal.Client.Application.Queries;
//using SFA.DAS.EAS.Portal.Client.Data;
//using SFA.DAS.EAS.Portal.Client.Database.Models;
//using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
//using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;
//using SFA.DAS.EAS.Portal.Client.Types;
//
//namespace SFA.DAS.EAS.Portal.UnitTests.Client.Services.DasRecruit
//{
//    public class DasRecruitServiceTests
//    {
//        private readonly GetAccountQuery _accountQuery;
//        private readonly Mock<IAccountsReadOnlyRepository> _accountsRepoMock;
//        private readonly Mock<IDasRecruitService> _dasRecruitService;
//        private readonly AccountDocument _testAccountDocument;
//        private VacanciesSummary _vancanciesSummaryResult;
//
//        public DasRecruitServiceTests()
//        {
//            _vancanciesSummaryResult = new VacanciesSummary
//            (
//                new List<VacancySummary>
//                {
//                    new VacancySummary
//                    {
//                        ApplicationMethod = "ThroughFindAnApprenticeship",
//                        ClosingDate = DateTime.Parse("10-10-2020 00:00:00"),
//                        CreatedDate = DateTime.Parse("12-06-2019 10:35:10.457"),
//                        Duration = 2,
//                        DurationUnit = "Year",
//                        EmployerAccountId = "ABC1D3",
//                        EmployerName = "Rosie's Boats Company",
//                        FaaVacancyDetailUrl = "https://findapprenticeship.service.gov.uk/apprenticeship/1000004431",
//                        LegalEntityId = 1234,
//                        LegalEntityName = "Rosie's Boats",
//                        Ukprn = 12345678,
//                        Status = "Live",
//                        ProgrammeId = "34",
//                        TrainingTitle = "Able seafarer (deck)",
//                        TrainingLevel = "Intermediate",
//                        TrainingType = "Standard",
//                        NoOfNewApplications = 0,
//                        NoOfUnsuccessfulApplications = 0,
//                        NoOfSuccessfulApplications = 1,
//                        VacancyReference = 1000004431,
//                        Title = "Seafarer apprenticeship",
//                        RaaManageVacancyUrl =
//                            "https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/",
//                        StartDate = DateTime.Parse("10-11-2019 00:00:00")
//                    }
//                },
//                25,
//                1,
//                1,
//                1
//            );
//
//            _testAccountDocument = new AccountDocument(1);
//
//            _dasRecruitService = new Mock<IDasRecruitService>();
//            _dasRecruitService.Setup(x => x.GetVacancies(It.IsAny<long>()))
//                .ReturnsAsync(_vancanciesSummaryResult);
//            _accountsRepoMock = new Mock<IAccountsReadOnlyRepository>();
//            _accountQuery = new GetAccountQuery(_accountsRepoMock.Object, _dasRecruitService.Object);
//            _accountsRepoMock.Setup(x => x.GetAccountDocumentById(It.IsAny<long>(), CancellationToken.None))
//                .ReturnsAsync(_testAccountDocument);
//
//        }
//
//        [Test]
//        public async Task WhenClientIsCalledAndGetRecruitIsTrue_ThenRecruitApiIsCalled()
//        {
//            //Act
//            var accountId = 1;
//            await _accountQuery.Get(accountId, true);
//
//            //Assert
//            _dasRecruitService.Verify(x => x.GetVacancies(accountId), Times.Once);
//        }
//
//        [Test]
//        public async Task WhenRecruitApiIsCalled_ThenDataIsConvertedCorrectly()
//        {
//            //Assign
//            var expectedVacancy = new Vacancy
//            {
//                ClosingDate = DateTime.Parse("10-10-2020 00:00:00"),
//                ManageVacancyUrl =
//                    "https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/",
//                NumberOfApplications = 1,
//                Reference = 1000004431,
//                Status = VacancyStatus.Live,
//                Title = "Seafarer apprenticeship",
//                TrainingTitle = "Able seafarer (deck)"
//            };
//            
//
//            //Act
//            var result = await _accountQuery.Get(1, true);
//
//            //Assert
//            result.Vacancies.Should().AllBeEquivalentTo(expectedVacancy);
//
//        }
//    }
//
//}
//
