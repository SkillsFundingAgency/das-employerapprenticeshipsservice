using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Recruit
{
    public class WhenIGetVacanciesWithTimeout
    {   
        private Mock<IHttpService> _mockHttpService;
        private Mock<IHttpServiceFactory> _mockHttpServiceFactory;
        private EmployerAccountsConfiguration _employerAccountsConfiguration;
        private Mock<IMapper> _mockMapper;
        private IAsyncPolicy _policy;
        private string _hashedAccountId;
        private string _apiBaseUrl;
        private string _clientId;
        private string _clientSecret;
        private string _identifierUri;
        private string _tenent;
        private string _serviceJson;
        private List<VacancySummary> _vacancies;
        private RecruitServiceWithTimeout _recruitServiceWithTimeout;
        
        [SetUp]
        public void Arrange()
        {
            _hashedAccountId = Guid.NewGuid().ToString();
            _apiBaseUrl = $"http://{Guid.NewGuid().ToString()}";
            _clientId = Guid.NewGuid().ToString();
            _clientSecret = Guid.NewGuid().ToString();
            _identifierUri = Guid.NewGuid().ToString();
            _tenent = Guid.NewGuid().ToString();
            _vacancies = new List<VacancySummary> { new VacancySummary()};
            _serviceJson = JsonConvert.SerializeObject(new VacanciesSummary { Vacancies = _vacancies });

            _mockHttpService = new Mock<IHttpService>();
            _mockHttpServiceFactory = new Mock<IHttpServiceFactory>();
            _employerAccountsConfiguration = new EmployerAccountsConfiguration
            {
                RecruitApi = new RecruitClientApiConfiguration
                {
                    ApiBaseUrl = _apiBaseUrl,
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    IdentifierUri = _identifierUri,
                    Tenant = _tenent
                }
            };
            _mockMapper = new Mock<IMapper>();

            _mockHttpService
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(_serviceJson);

            _mockHttpServiceFactory
                .Setup(m => m.Create(_clientId, _clientSecret, _identifierUri, _tenent))
                .Returns(_mockHttpService.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(It.IsAny<List<VacancySummary>>()))
                .Returns(new List<Vacancy>());

            _policy = Policy.NoOpAsync();
            var registryPolicy = new PolicyRegistry();
            registryPolicy.Add(Constants.DefaultServiceTimeout, _policy);

            _recruitServiceWithTimeout = new RecruitServiceWithTimeout(_mockHttpServiceFactory.Object, 
                _employerAccountsConfiguration,
                _mockMapper.Object, registryPolicy);
        }

        [Test]
        public async Task ThenTheRecruitServiceIsCalled()
        {
            //arrange
            var expectedUrl = $"{_apiBaseUrl}/api/vacancies?employerAccountId={_hashedAccountId}&pageSize={int.MaxValue}";

            //act
             await _recruitServiceWithTimeout.GetVacancies(_hashedAccountId);

            // assert 
            _mockHttpService.Verify(x => x.GetAsync(expectedUrl,false), Times.Once);
        }

        [Test]
        public async Task ThenThrowTimeoutException()
        {
            var expectedUrl = $"{_apiBaseUrl}/api/vacancies?employerAccountId={_hashedAccountId}&pageSize={int.MaxValue}";
           
            var innerException = "Exception of type 'Polly.Timeout.TimeoutRejectedException' was thrown.";
            var message = "Call to Recruit Service timed out";
            Exception actualException = null;
            var correctExceptionThrown = false;

            try
            {
                _mockHttpService.Setup(p => p.GetAsync(expectedUrl, false))
                    .Throws<TimeoutRejectedException>();
                await _recruitServiceWithTimeout.GetVacancies(_hashedAccountId);
            }
            catch (Exception e)
            {
                actualException = e;
                correctExceptionThrown = true;
            }
            Assert.IsTrue(correctExceptionThrown);
            Assert.AreEqual(actualException.InnerException?.Message, innerException);
            Assert.AreEqual(actualException.Message, message);
        }
    }
}
