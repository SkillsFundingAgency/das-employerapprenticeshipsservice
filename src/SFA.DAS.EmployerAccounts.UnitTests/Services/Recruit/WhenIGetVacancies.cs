using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Recruit
{
    public class GetVacancies
    {  
        private RecruitClientApiConfiguration _recruitClientApiConfiguration;
        private Mock<IMapper> _mockMapper;
        Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private string _testData;

        private string _hashedAccountId;
        private string _apiBaseUrl;
        private string _clientId;
        private string _clientSecret;
        private string _identifierUri;
        private string _tenent;
        private string _serviceJson;
        private List<VacancySummary> _vacancies;

        private RecruitService _sut;

        [SetUp]
        public void Arrange()
        {
            ConfigurationManager.AppSettings["EnvironmentName"] = "LOCAL";
            _hashedAccountId = Guid.NewGuid().ToString();
            _apiBaseUrl = $"http://{Guid.NewGuid().ToString()}";
            _clientId = Guid.NewGuid().ToString();
            _clientSecret = Guid.NewGuid().ToString();
            _identifierUri = Guid.NewGuid().ToString();
            _tenent = Guid.NewGuid().ToString();
            _vacancies = new List<VacancySummary> { new VacancySummary { } };
            _serviceJson = JsonConvert.SerializeObject(new VacanciesSummary { Vacancies = _vacancies });          

            _recruitClientApiConfiguration = new RecruitClientApiConfiguration
            {
                ApiBaseUrl = _apiBaseUrl,
                IdentifierUri = _identifierUri
            };

            _mockMapper = new Mock<IMapper>();
            
            _mockMapper
                .Setup(m => m.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(It.IsAny<List<VacancySummary>>()))
                .Returns(new List<Vacancy>());


            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _testData = _serviceJson;

            _mockHttpMessageHandler
                  .Protected()
                  .Setup<Task<HttpResponseMessage>>("SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                  .ReturnsAsync(new HttpResponseMessage
                  {
                      Content = new StringContent(_testData),
                      StatusCode = HttpStatusCode.OK
                  }
                  ).Verifiable("");

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _sut = new RecruitService(httpClient, _recruitClientApiConfiguration, _mockMapper.Object);
        }

        [Test]
        public async Task ThenTheServiceIsCalled()
        {
            //Arrange
            var expectedUrl = $"{_apiBaseUrl}/api/vacancies?employerAccountId={_hashedAccountId}&pageSize={int.MaxValue}";

            //Act
            await _sut.GetVacancies(_hashedAccountId);

            //Assert
            _mockHttpMessageHandler
               .Protected()
               .Verify("SendAsync", Times.Once(),
                   ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                      && r.RequestUri.AbsoluteUri == $"{expectedUrl}"),
               ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task ThenTheServiceDataIsMapped()
        {
            //Act
            await _sut.GetVacancies(_hashedAccountId);

            //Assert
            _mockMapper.Verify(m => m.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(It.IsAny<List<VacancySummary>>()), Times.Once);
        }

        [Test]
        public async Task ThenTheMappedVacancyDataIsReturned()
        {
            //Arrange
            var testTitle = Guid.NewGuid().ToString();

            var vacancies = new List<Vacancy>
            {
                new Vacancy { Title = testTitle }
            };

            _mockMapper
                .Setup(m => m.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(It.IsAny<List<VacancySummary>>()))
                .Returns(vacancies);

            //Act
            var result = await _sut.GetVacancies(_hashedAccountId) as List<Vacancy>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testTitle, result[0].Title);
        }
    }
}
