using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.UnitTests.Fakes;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Client.Services.DasRecruit
{
    [TestFixture, Parallelizable]
    internal class DasRecruitServiceTests : FluentTest<DasRecruitServiceTestsFixture>
    {
        [Test]
        public Task GetVacancies_WhenSingleVacancyIsReturnedByApi_ThenCorrectlyMappedVacancyIsReturned()
        {
            return TestAsync(f => f.ArrangeApiReturnsOk(),
                f => f.GetVacancies(),
                (f, r) => f.AssertVacancies(r));
        }

        [Test]
        public Task GetVacancies_WhenApiReturnsAnError_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeApiReturnsInternalServerError(),
                f => f.GetVacancies(),
                (f, r) => f.AssertNullReturned(r));
        }
    }

    internal class DasRecruitServiceTestsFixture
    {
        public DasRecruitService DasRecruitService { get; set; }
        public Mock<IRecruitApiHttpClientFactory> RecruitApiHttpClientFactory { get; set; }
        public FakeHttpMessageHandler HttpMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
        public Mock<ILog> Log { get; set; }
        
        public DasRecruitServiceTestsFixture()
        {
            HttpMessageHandler = new FakeHttpMessageHandler();
            HttpClient = new HttpClient(HttpMessageHandler) { BaseAddress = new Uri("https://example.com") };

            RecruitApiHttpClientFactory = new Mock<IRecruitApiHttpClientFactory>();
            RecruitApiHttpClientFactory.Setup(f => f.CreateHttpClient()).Returns(HttpClient);

            Log = new Mock<ILog>();

            DasRecruitService = new DasRecruitService(RecruitApiHttpClientFactory.Object, Log.Object);
        }

        public DasRecruitServiceTestsFixture ArrangeApiReturnsOk()
        {
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
            const string hashedAccountId = "HASHAI";
            return await DasRecruitService.GetVacancies(hashedAccountId);
        }

        public void AssertVacancies(IEnumerable<Vacancy> vacancies)
        {
            vacancies.Should().BeEquivalentTo(new List<Vacancy>
            {
                new Vacancy
                {
                    Title = "Seafarer apprenticeship",
                    Reference = 1000004431L,
                    Status = VacancyStatus.Live,
                    ClosingDate = new DateTime(2020, 10, 10), 
                    TrainingTitle = "Able seafarer (deck)",
                    NumberOfApplications = 1,
                    ManageVacancyUrl = "https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/"
                }
            });
        }

        public void AssertNullReturned(IEnumerable<Vacancy> vacancies)
        {
            vacancies.Should().BeNull();
        }
    }
}