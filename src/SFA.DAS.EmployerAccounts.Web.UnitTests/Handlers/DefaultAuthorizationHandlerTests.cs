using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using AuthorizationContext = SFA.DAS.Authorization.Context.AuthorizationContext;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers
{
    [TestFixture]
    public class DefaultAuthorizationHandlerTests
    {
        public List<string> Options { get; set; }
        public IAuthorizationContext AuthorizationContext { get; set; }
        public IAuthorizationContextProvider AuthorizationContextProvider { get; set; }
        public SFA.DAS.EmployerAccounts.Web.Authorization.DefaultAuthorizationHandler SutDefaultAuthorizationHandler { get; set; }
        public Mock<HttpContextBase> MockHttpContextBase { get; set; }
        public Mock<System.Security.Principal.IIdentity> MockUser { get; set; }
        public Mock<IEmployerAccountTeamRepository> MockEmployerAccountTeamRepository { get; set; }
        public Mock<IAuthorizationContextProvider> MockAuthorizationContextProvider { get; set; }
        public AuthorizationContextTestsFixture AuthorizationContextTestsFixture { get; set; }
       

        [SetUp]
        public void Arrange()
        {
            AuthorizationContextTestsFixture = new AuthorizationContextTestsFixture();
            Options = new List<string>();            
            SutDefaultAuthorizationHandler = new SFA.DAS.EmployerAccounts.Web.Authorization.DefaultAuthorizationHandler();
            MockHttpContextBase = new Mock<HttpContextBase>();
            MockUser = new Mock<System.Security.Principal.IIdentity>();            
            MockEmployerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
            MockAuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
            AuthorizationContextProvider = new ImpersonationAuthorizationContext(MockHttpContextBase.Object, MockAuthorizationContextProvider.Object, MockEmployerAccountTeamRepository.Object);
            AuthorizationContext = new AuthorizationContext();            
        }

        [Test]
        public async Task GetAuthorizationResult_WhenTheUserIsTier2_ThenApply()
        {
            //Act
            var authorizationResult = await  SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContext);

            //Assert
            authorizationResult.IsAuthorized.Should().Be(true);
        }

        [Test]
        public void GetAuthorizationResult_AuthorizationContextn()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetTestData(3);

            //Act   
            var result =  AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            result.Should().Be("Foo_0: Bar_0, Foo_1: Bar_1, Foo_2: Bar_2");
        }

        [Test]
        public void Test_AuthorizationContext()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData();

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult =  SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
        }

    }


    public class AuthorizationContextTestsFixture
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueOut { get; set; }
        public IAuthorizationContext AuthorizationContext { get; set; }
        protected Mock<HttpContextBase> MockContextBase;
        protected Mock<HttpRequestBase> MockRequestBase;
        protected Mock<HttpResponseBase> MockResponseBase;
        protected Mock<RouteBase> MockRouteBase;
        protected Mock<RouteData> MockRouteData;
        public Mock<IRouteHandler> MockRouteHandler { get; set; }
        public Mock<ClaimsIdentity> MockClaimsIdentity { get; set; }
        public Mock<System.Security.Principal.IIdentity> MockUser { get; set; }


        public AuthorizationContextTestsFixture()
        {
            Key = "Foo";
            Value = "Bar";
            AuthorizationContext = new AuthorizationContext();
        }

        public string GetData()
        {
            return AuthorizationContext.Get<string>(Key);
        }

        public AuthorizationContextTestsFixture SetTestData(int count = 1)
        {
            if (count == 1)
            {
                AuthorizationContext.Set(Key, Value);
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    AuthorizationContext.Set($"{Key}_{i}", $"{Value}_{i}");
                }
            }

            return this;
        }


        public AuthorizationContextTestsFixture SetData()        {
          
        RouteData routeData = new RouteData();
        routeData.Values.Add("RouteData", "accounts/{hashedaccountid}/teams/view");

            MockClaimsIdentity = new Mock<ClaimsIdentity>();
            MockContextBase = new Mock<HttpContextBase>();
            MockRequestBase = new Mock<HttpRequestBase>();
            MockResponseBase = new Mock<HttpResponseBase>();
            MockRouteBase = new Mock<RouteBase>();
            MockRouteData = new Mock<RouteData>();
            MockRouteHandler = new Mock<IRouteHandler>();
            var routebase = new Route("accounts/{hashedaccountid}/teams/view", MockRouteHandler.Object);
            MockUser = new Mock<System.Security.Principal.IIdentity>();

            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);          

            var routeData123 = new RouteData(routebase, MockRouteHandler.Object);
            AuthorizationContext.Set("RouteData", routeData123);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),                
            });
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, "Tier2User"));

            var principal = new ClaimsPrincipal(claimsIdentity);
            
            MockContextBase.Setup(c => c.User).Returns(principal);

            /**************/
            //MockContextBase.Setup(c => c.User.IsInRole("Tier2")).Returns(true);
            //MockClaimsIdentity.Setup(x => x.RoleClaimType).Returns("Tier2");
            //MockClaimsIdentity.Setup(x => x.AddClaim(new Claim("sub", "UserRef")));
            //MockClaimsIdentity.Setup(x => x.AddClaim(new Claim(DasClaimTypes.Id, "UserRef")));
            //MockClaimsIdentity.Setup(x => x.AddClaim(new Claim(DasClaimTypes.Email, "Email")));
            //var principal1 = new ClaimsPrincipal(MockClaimsIdentity.Object);
            //MockContextBase.Setup(c => c.User).Returns(principal1);
            //var roleClaims = new[] { "Admin", "Local" };
            /**************/


            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);

            return this;
        }

        public bool TryGetData()
        {
            var exists = AuthorizationContext.TryGet(Key, out string value);

            ValueOut = value;

            return exists;
        }
    }

}
