using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.UnitTests.Fakes;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Client.Services.DasRecruit
{
    [TestFixture, Parallelizable]
    internal class DasRecruitServiceTests : FluentTest<DasRecruitServiceTestsFixture>
    {
        [Test]
        public Task GetVacancies_X_y()
        {
            return TestAsync(f => f.ArrangeApiReturnsOk(), f => f.GetVacancies(), (f, r) => r.Should());
        }
    }

    internal class DasRecruitServiceTestsFixture
    {
        public DasRecruitService DasRecruitService { get; set; }
        public ILogger<DasRecruitService> Logger { get; set; }
        public Mock<IRecruitApiHttpClientFactory> RecruitApiHttpClientFactory { get; set; }
        public FakeHttpMessageHandler HttpMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
        
        public DasRecruitServiceTestsFixture()
        {
            Logger = new NullLogger<DasRecruitService>();

            HttpMessageHandler = new FakeHttpMessageHandler();
            HttpClient = new HttpClient(HttpMessageHandler) { BaseAddress = new Uri("https://example.com") };

            RecruitApiHttpClientFactory = new Mock<IRecruitApiHttpClientFactory>();
            RecruitApiHttpClientFactory.Setup(f => f.CreateHttpClient()).Returns(HttpClient);
            
            DasRecruitService = new DasRecruitService(RecruitApiHttpClientFactory.Object, Logger);
        }

        public DasRecruitServiceTestsFixture ArrangeApiReturnsOk()
        {
//            var vs = new VacanciesSummary(new List<VacancySummary> {{new VacancySummary
//                    {
//                    ApplicationMethod = "ThroughFindAnApprenticeship",
//                    ClosingDate = DateTime.Parse("10-10-2020 00:00:00"),
//                    CreatedDate = DateTime.Parse("12-06-2019 10:35:10.457"),
//                    Duration = 2,
//                    DurationUnit = "Year",
//                    EmployerAccountId = "ABC1D3",
//                    EmployerName = "Rosie's Boats Company",
//                    FaaVacancyDetailUrl = "https://findapprenticeship.service.gov.uk/apprenticeship/1000004431",
//                    LegalEntityId = 1234,
//                    LegalEntityName = "Rosie's Boats",
//                    Ukprn = 12345678,
//                    Status = "Live",
//                    ProgrammeId = "34",
//                    TrainingTitle = "Able seafarer (deck)",
//                    TrainingLevel = "Intermediate",
//                    TrainingType = "Standard",
//                    NoOfNewApplications = 0,
//                    NoOfUnsuccessfulApplications = 0,
//                    NoOfSuccessfulApplications = 1,
//                    VacancyReference = 1000004431,
//                    Title = "Seafarer apprenticeship",
//                    RaaManageVacancyUrl =
//                        "https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/",
//                    StartDate = DateTime.Parse("10-11-2019 00:00:00")
//                }}}, 25, 1, 1, 1);
//
//            var jj = JsonConvert.SerializeObject(vs);
            
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Vacancies\":[{\"Title\":\"Seafarer apprenticeship\",\"VacancyReference\":1000004431,\"LegalEntityId\":1234,\"LegalEntityName\":\"Rosie's Boats\",\"EmployerAccountId\":\"ABC1D3\",\"EmployerName\":\"Rosie's Boats Company\",\"Ukprn\":12345678,\"CreatedDate\":\"2019-06-12T10:35:10.457\",\"Status\":\"Live\",\"ClosingDate\":\"2020-10-10T00:00:00\",\"Duration\":2,\"DurationUnit\":\"Year\",\"ApplicationMethod\":\"ThroughFindAnApprenticeship\",\"ProgrammeId\":\"34\",\"StartDate\":\"2019-11-10T00:00:00\",\"TrainingTitle\":\"Able seafarer (deck)\",\"TrainingType\":\"Standard\",\"TrainingLevel\":\"Intermediate\",\"NoOfNewApplications\":0,\"NoOfSuccessfulApplications\":1,\"NoOfUnsuccessfulApplications\":0,\"FaaVacancyDetailUrl\":\"https://findapprenticeship.service.gov.uk/apprenticeship/1000004431\",\"RaaManageVacancyUrl\":\"https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/\"}],\"PageSize\":25,\"PageNo\":1,\"TotalResults\":1,\"TotalPages\":1}"),
                ReasonPhrase = "OK",
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.com")
            };
            
            return this;
        }

        public DasRecruitServiceTestsFixture ArrangeApiReturnsInternalServerError()
        {
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Kaboom"),
                ReasonPhrase = "Internal server error",
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.com")
            };
            
            return this;
        }
        
        public async Task<IEnumerable<Vacancy>> GetVacancies()
        {
            const long accountId = 123L;
            return await DasRecruitService.GetVacancies(accountId);
        }
    }
}