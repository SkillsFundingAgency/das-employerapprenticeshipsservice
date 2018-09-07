﻿using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetGatewayInformation;
using SFA.DAS.EAS.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.HmrcOrchestratorTests
{
    public class WhenCallingHmrcService
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<ILog> _logger;
        private Mock<IMediator> _mediator;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILog>();
            _mediator = new Mock<IMediator>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration()
            };

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration, Mock.Of<IHashingService>());   
        }

        [Test]
        public async Task ThenTheHmrcServiceIsCalled()
        {
            //Arrange
            var redirectUrl = "myUrl";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayInformationQuery>())).ReturnsAsync(new GetGatewayInformationResponse {Url = "someurl"});

            //Act
            await _employerAccountOrchestrator.GetGatewayUrl(redirectUrl);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetGatewayInformationQuery>(c=>c.ReturnUrl.Equals(redirectUrl))));
        }

        [Test]
        public async Task ThenICanRetrieveEmployerInformationWithMyAccessToken()
        {
            //Arrange
            var expectedAuthToken = "123";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>())).ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation()});

            //Act
            await _employerAccountOrchestrator.GetHmrcEmployerInformation(expectedAuthToken, string.Empty);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c => c.AuthToken.Equals(expectedAuthToken))));
        }

        [Test]
        public async Task ThenICanRetrieveCorrectEmpRefOfScenarioUser()
        {
            //Arrange
            var scenarioUserEmail = "test.user@test.com";
            var expectedEmpRef = "123/456789";

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>()))
                .ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation(), Empref = expectedEmpRef});
          
            //Act
            var result = await _employerAccountOrchestrator.GetHmrcEmployerInformation("123", scenarioUserEmail);
            
            //Assert
            Assert.AreEqual(expectedEmpRef, result.Empref);
        }

        [Test]
        public async Task ThenIfANotFoundExceptionIsThrownThePropertyIsSetOnTheResponse()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>())).ThrowsAsync(new NotFoundException("Empref not found"));

            //Act
            var result = await _employerAccountOrchestrator.GetHmrcEmployerInformation("123", "test@test.com");

            //Assert
            Assert.IsTrue(result.EmprefNotFound);
        }
    }
}
