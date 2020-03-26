using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.GetVacancies
{
    public class GetVacancies
    {   
        private Mock<IHttpService> _mockHttpService;
        private Mock<IHttpServiceFactory> _mockHttpServiceFactory;
        private EmployerAccountsConfiguration _employerAccountsConfiguration;
        private Mock<IMapper> _mockMapper;

        private string _hashedAccountId;
        private string _apiBaseUrl;
        private string _clientId;
        private string _clientSecret;
        private string _identifierUri;
        private string _tenent;
        private string _serviceJson;
        private RecruitClientApiConfiguration _recruitClientApiConfiguration;
        private List<VacancySummary> _vacancies;

        private RecruitService _sut;

        [SetUp]
        public void Arrange()
        {
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
                 ClientId = _clientId,
                 ClientSecret = _clientSecret,
                 IdentifierUri = _identifierUri,
                 Tenant = _tenent
            };

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

            _mockHttpServiceFactory
                .Setup(m => m.Create(_clientId, _clientSecret, _identifierUri, _tenent))
                .Returns(_mockHttpService.Object);

            _mockHttpService
              .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<bool>()))
              .ReturnsAsync(_serviceJson);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(It.IsAny<List<VacancySummary>>()))
                .Returns(new List<Vacancy>());

            _sut = new RecruitService(_mockHttpServiceFactory.Object, _employerAccountsConfiguration, _mockMapper.Object);
        }

        [Test]
        public async Task ThenTheServiceIsCalled()
        {
            //Arrange
            var expectedUrl = $"{_apiBaseUrl}/api/vacancies?employerAccountId={_hashedAccountId}&pageSize={int.MaxValue}";

            //Act
            await _sut.GetVacancies(_hashedAccountId);

            //Assert
            _mockHttpService.Verify(x => x.GetAsync(expectedUrl, false), Times.Once);
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
