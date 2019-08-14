using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.UnitTests.Fakes;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Features
{
    [TestFixture]
    public class DasRecruitServiceTests : FluentTest<DasRecruitServiceTestsFixture>
    {
       [Test]
        public Task GetVacancies_WhenSingleVacancyIsReturnedByApi_ThenCorrectlyMappedVacancyIsReturned()
        {
            return RunAsync(
                f => f.ArrangeApiReturnsOk(),
                f => f.GetVacancies(),
                (f,r) => f.AssertVacancies(r));
        }

        [Test]
        public Task GetVacancies_WhenApiReturnsNotFound_ThenEmptyEnumerableIsReturned()
        {
            return RunAsync(f => f.ArrangeApiReturnsNotFound(),
                f => f.GetVacancies(),
                (f, r) => f.AssertNoVacancies(r));
        }

        [Test]
        public Task GetVacancies_WhenApiReturnsAnError_ThenNullIsReturned()
        {
            return RunAsync(f => f.ArrangeApiReturnsInternalServerError(),
                f => f.GetVacancies(),
                (f, r) => f.AssertNullReturned(r));
        }
    }

    public class DasRecruitServiceTestsFixture : FluentTestFixture
    {
        public DasRecruitService DasRecruitService { get; set; }
        public Mock<IRecruitApiHttpClientFactory> RecruitApiHttpClientFactory { get; set; }
        public FakeHttpMessageHandler HttpMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
        private const int LegalEntityId = 1234;
        public Mock<ILog> Log { get; set; }
        private const string ServiceBaseUrl = "http://example.com";
        private Vacancy Vacancy => new Vacancy
        {
            Title = "Seafarer apprenticeship",
            VacancyReference = 1000004431L,
            Status = "Live",
            ClosingDate = new DateTime(2020, 10, 10),
            TrainingTitle = "Able seafarer (deck)",
            LegalEntityId = LegalEntityId

        };
        IEnumerable<Vacancy> Vacancies { get; set; }
        public DasRecruitServiceTestsFixture()
        {
            HttpMessageHandler = new FakeHttpMessageHandler();
            HttpClient = new HttpClient(HttpMessageHandler) {BaseAddress = new Uri(ServiceBaseUrl) };
            RecruitApiHttpClientFactory = new Mock<IRecruitApiHttpClientFactory>();
            RecruitApiHttpClientFactory.Setup(f => f.CreateHttpClient()).Returns(HttpClient);

            Log = new Mock<ILog>();

            DasRecruitService = new DasRecruitService(RecruitApiHttpClientFactory.Object, Log.Object);
        }
        public async Task<VacanciesSummary> GetVacancies()
        {
            const string hashedAccountId = "HASHAI";
            return await DasRecruitService.GetVacanciesByLegalEntity(hashedAccountId, LegalEntityId);
        }

        public DasRecruitServiceTestsFixture ArrangeApiReturnsOk()
        {
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Vacancies\":[{\"Title\":\"Seafarer apprenticeship\",\"VacancyReference\":1000004431,\"LegalEntityId\":1234,\"LegalEntityName\":\"Rosie's Boats\",\"EmployerAccountId\":\"ABC1D3\",\"EmployerName\":\"Rosie's Boats Company\",\"Ukprn\":12345678,\"CreatedDate\":\"2019-06-12T10:35:10.457\",\"Status\":\"Live\",\"ClosingDate\":\"2020-10-10T00:00:00\",\"Duration\":2,\"DurationUnit\":\"Year\",\"ApplicationMethod\":\"ThroughFindAnApprenticeship\",\"ProgrammeId\":\"34\",\"StartDate\":\"2019-11-10T00:00:00\",\"TrainingTitle\":\"Able seafarer (deck)\",\"TrainingType\":\"Standard\",\"TrainingLevel\":\"Intermediate\",\"NoOfNewApplications\":0,\"NoOfSuccessfulApplications\":1,\"NoOfUnsuccessfulApplications\":0,\"FaaVacancyDetailUrl\":\"https://findapprenticeship.service.gov.uk/apprenticeship/1000004431\",\"RaaManageVacancyUrl\":\"https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/\"}],\"PageNo\":1,\"TotalResults\":1,\"TotalPages\":1}"),
                ReasonPhrase = "OK",
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, ServiceBaseUrl)
            };
            return this;
        }

        public DasRecruitServiceTestsFixture ArrangeApiReturnsNotFound()
        {
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Not found"),
                ReasonPhrase = "Not found",
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, ServiceBaseUrl)
            };

            return this;
        }

        public DasRecruitServiceTestsFixture ArrangeApiReturnsInternalServerError()
        {
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Kaboom"),
                ReasonPhrase = "Internal server error",
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, ServiceBaseUrl)
            };

            return this;
        }

        public void AssertVacancies(VacanciesSummary summary)
        {
            Vacancies = Enumerable.Repeat(Vacancy, 1);
            VacanciesSummary vacanciesSummary=new VacanciesSummary(Vacancies,1,1,1,1);
            summary.Vacancies.Count().Should().Be(vacanciesSummary.Vacancies.Count());
            summary.Vacancies.First().VacancyReference.ShouldBeEquivalentTo(vacanciesSummary.Vacancies.First().VacancyReference);
            summary.Vacancies.All(v => v.LegalEntityId == vacanciesSummary.Vacancies.FirstOrDefault().LegalEntityId);
        }

        public void AssertNoVacancies(VacanciesSummary summary)
        {
            summary.Vacancies.Should().BeNull(); 
        }

        public void AssertNullReturned(VacanciesSummary summary)
        {
            summary.Should().BeNull();
        }
    }
}
