using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.DasRecruitService.Models;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Infrastructure.Configuration;

namespace SFA.DAS.EAS.DasRecruitService.UnitTests
{
    public class DasRecruitServiceTests
    {
        private readonly GetAccountQuery _accountQuery;
        private readonly Mock<IAccountsReadOnlyRepository> _accountsRepoMock;
        private readonly Mock<Services.IDasRecruitService> _dasRecruitService;
        private readonly AccountDocument _testAccountDocument;
        private VacanciesSummary _vancanciesSummaryResult;

        public DasRecruitServiceTests()
        {
            _vancanciesSummaryResult = new VacanciesSummary
            (
                new List<VacancySummary>
                {
                    new VacancySummary
                    {
                        ApplicationMethod = "ThroughFindAnApprenticeship",
                        ClosingDate = DateTime.Parse("10-10-2020 00:00:00"),
                        CreatedDate = DateTime.Parse("12-06-2019 10:35:10.457"),
                        Duration = 2,
                        DurationUnit = "Year",
                        EmployerAccountId = "ABC1D3",
                        EmployerName = "Rosie's Boats Company",
                        FaaVacancyDetailUrl = "https://findapprenticeship.service.gov.uk/apprenticeship/1000004431",
                        LegalEntityId = 1234,
                        LegalEntityName = "Rosie's Boats",
                        Ukprn = 12345678,
                        Status = "Live",
                        ProgrammeId = "34",
                        TrainingTitle = "Able seafarer (deck)",
                        TrainingLevel = "Intermediate",
                        TrainingType = "Standard",
                        NoOfNewApplications = 0,
                        NoOfUnsuccessfulApplications = 0,
                        NoOfSuccessfulApplications = 1,
                        VacancyReference = 1000004431,
                        Title = "Seafarer apprenticeship",
                        RaaManageVacancyUrl = "https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/",
                        StartDate = DateTime.Parse("10-11-2019 00:00:00")
                    }
                },
                25,
                1,
                1,
                1
            );

            _testAccountDocument = new AccountDocument(1);

            _dasRecruitService = new Mock<Services.IDasRecruitService>();
            _dasRecruitService.Setup(x => x.GetVacanciesSummary(It.IsAny<long>()))
                .ReturnsAsync(_vancanciesSummaryResult);
            _accountsRepoMock = new Mock<IAccountsReadOnlyRepository>();
            _accountQuery = new GetAccountQuery(_accountsRepoMock.Object,_dasRecruitService.Object);
            _accountsRepoMock.Setup(x => x.GetAccountDocumentById(It.IsAny<long>(),CancellationToken.None)).ReturnsAsync(_testAccountDocument);

        }

        [Test]
        public async Task WhenClientIsCalledAndGetRecruitIsTrue_ThenRecruitApiIsCalled()
        {
            //Act
            var accountId = 1;
            await _accountQuery.Get(accountId, true);

            //Assert
            _dasRecruitService.Verify(x => x.GetVacanciesSummary(accountId),Times.Once);
        }
    }
}
