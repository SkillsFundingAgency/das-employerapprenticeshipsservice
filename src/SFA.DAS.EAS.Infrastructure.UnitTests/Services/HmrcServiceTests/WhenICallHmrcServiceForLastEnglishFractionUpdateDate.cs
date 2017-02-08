﻿using System;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    class WhenICallHmrcServiceForLastEnglishFractionUpdateDate
    {
        private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private const string ExpectedClientId = "654321";
        private const string ExpectedScope = "emp_ref";
        private const string ExpectedClientSecret = "my_secret";
        private const string ExpectedTotpToken = "789654321AGFVD";

        private HmrcService _hmrcService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private Mock<ITotpService> _totpService;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration
                {
                    BaseUrl = ExpectedBaseUrl,
                    ClientId = ExpectedClientId,
                    Scope = ExpectedScope,
                    ClientSecret = ExpectedClientSecret,
                    ServerToken = "token1234"
                }
            };
            
            _httpClientWrapper = new Mock<IHttpClientWrapper>();

            _totpService = new Mock<ITotpService>();
            _totpService.Setup(x => x.GetCode(It.IsAny<string>())).Returns(ExpectedTotpToken);

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _totpService.Object);
        }

        [Test]
        public async Task ThenIShouldGetTheCurrentUpdatedDate()
        {
            //Assign
            const string expectedApiUrl = "apprenticeship-levy/fraction-calculation-date";
            var updateDate = DateTime.Now;
            
            _httpClientWrapper.Setup(x => x.Get<DateTime>(It.IsAny<string>(), expectedApiUrl))
                .ReturnsAsync(updateDate);

            //Act
            var result = await _hmrcService.GetLastEnglishFractionUpdate();

            //Assert
            _httpClientWrapper.Verify(x => x.Get<DateTime>(_configuration.Hmrc.ServerToken, expectedApiUrl), Times.Once);
            Assert.AreEqual(updateDate, result);
        }
    }
}
