using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Extensions;
using SFA.DAS.EAS.Support.Web.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EAS.Support.Web.Tests.Extensions
{
    [TestFixture]
    public class WhenTestingUrlHelperExtensions
    {
        private WebConfiguration _configuration;
        private Mock<IDependencyResolver> _dependancyResolver;
        protected AccountController Controller;
        private Mock<IAccountHandler> _accountHandler;
        private Mock<IPayeLevySubmissionsHandler> _payeLevySubmissionsHandler;
        private Mock<ILog> _log;
        private Mock<IPayeLevyMapper> _payeLevyMapper;
        private Mock<HttpContextBase> _httpContextMock;

        [SetUp]
        public void Setup()
        {
            _accountHandler=new Mock<IAccountHandler>();
            _payeLevySubmissionsHandler=new Mock<IPayeLevySubmissionsHandler>();
            _log = new Mock<ILog>();
            _payeLevyMapper = new Mock<IPayeLevyMapper>();
            _httpContextMock = new Mock<HttpContextBase>();
            _configuration = new WebConfiguration
            {
                AccountApi = new AccountApiConfiguration
                {
                    ApiBaseUrl = "--- configuration value goes here ---",
                    ClientId = "12312312-140e-4f9f-807b-112312312375",
                    ClientSecret = "--- configuration value goes here ---",
                    IdentifierUri = "--- configuration value goes here ---",
                    Tenant = "--- configuration value goes here ---"
                },
                SiteValidator = new SiteValidatorSettings
                {
                    Audience = "--- configuration value goes here ---",
                    Scope = "--- configuration value goes here ---",
                    Tenant = "--- configuration value goes here ---"
                },
                LevySubmission = new LevySubmissionsSettings
                {
                    HmrcApiBaseUrlSetting = new HmrcApiBaseUrlConfig
                    {
                        HmrcApiBaseUrl = "--- configuration value goes here ---"
                    },
                    LevySubmissionsApiConfig = new LevySubmissionsApiConfiguration
                    {
                        ApiBaseUrl = "",
                        ClientId = "",
                        ClientSecret = "",
                        IdentifierUri = "",
                        Tenant = "",
                        LevyTokenCertificatethumprint = ""
                    }
                },
                HashingService = new HashingServiceConfig
                {
                    AllowedCharacters = "",
                    Hashstring = ""
                },
                EmployerAccountsConfiguration = new EmployerAccountsConfiguration
                {
                    EmployerAccountsBaseUrl = "https://localhost:44344"
                }
            };
            _dependancyResolver=new Mock<IDependencyResolver>();
            _dependancyResolver.Setup(r => r.GetService(typeof(IWebConfiguration))).Returns(_configuration);
            DependencyResolver.SetResolver(_dependancyResolver.Object);

            var accountResponse = new AccountReponse()
            {
                Account = new Core.Models.Account(),
                StatusCode = SearchResponseCodes.Success
            };

            _accountHandler.Setup(a => a.FindTeamMembers("123")).ReturnsAsync(accountResponse);

            Controller = new AccountController(_accountHandler.Object,_payeLevySubmissionsHandler.Object,_log.Object,_payeLevyMapper.Object, _httpContextMock.Object);
            
            var routes = new RouteCollection();
           
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            
            var routeData = new RouteData();
            
            Controller.ControllerContext = new ControllerContext(context.Object, routeData, Controller);
            Controller.Url = new UrlHelper(new RequestContext(context.Object, routeData), routes);
        }

        [Test]
        public void StaffLoginAction_MustReturn_ExpectedRouteUrl()
        {
            var expected = "https://localhost:44344/login/staff?HashedAccountId=A1B2C3";
            var actual = UrlHelperExtensions.StaffLoginAction(Controller.Url, "login/staff?HashedAccountId=A1B2C3");

            Assert.AreEqual(expected, actual);
        }
    }
}
