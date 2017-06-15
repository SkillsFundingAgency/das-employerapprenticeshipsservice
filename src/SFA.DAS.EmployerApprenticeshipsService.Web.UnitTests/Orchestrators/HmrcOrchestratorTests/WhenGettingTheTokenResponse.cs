using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetGatewayToken;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.HmrcOrchestratorTests
{
    public class WhenGettingTheTokenResponse 
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
                Hmrc = new HmrcConfiguration ()
            };
            new Mock<IEmpRefFileBasedService>();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheTokenIsRetrievedFromTheQuery()
        {
            //Arrange
            var accessCode = "546tg";
            var returnUrl = "http://someUrl";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayTokenQuery>()))
                .ReturnsAsync(new GetGatewayTokenQueryResponse {HmrcTokenResponse = new HmrcTokenResponse()});

            //Act
            var token = await _employerAccountOrchestrator.GetGatewayTokenResponse(accessCode, returnUrl, null);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is< GetGatewayTokenQuery>(c=>c.AccessCode.Equals(accessCode) && c.RedirectUrl.Equals(returnUrl))));
            Assert.IsAssignableFrom<HmrcTokenResponse>(token.Data);
        }

        [Test]
        public async Task ThenTheFlashMessageIsPopulatedWhenAuthorityIsNotGranted()
        {
            //Act
            var actual = await _employerAccountOrchestrator.GetGatewayTokenResponse(string.Empty, string.Empty, new NameValueCollection { new NameValueCollection { {"error", "USER_DENIED_AUTHORIZATION" }, { "error_Code", "USER_DENIED_AUTHORIZATION" } } });

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<HmrcTokenResponse>>(actual);
            Assert.AreEqual("Account not added", actual.FlashMessage.Headline);
            Assert.AreEqual("error-summary", actual.FlashMessage.SeverityCssClass);
            Assert.AreEqual(FlashMessageSeverityLevel.Error, actual.FlashMessage.Severity);
            Assert.Contains(new KeyValuePair<string,string>("agree_and_continue", "Agree and continue"), actual.FlashMessage.ErrorMessages);
            
        }
    }
}
