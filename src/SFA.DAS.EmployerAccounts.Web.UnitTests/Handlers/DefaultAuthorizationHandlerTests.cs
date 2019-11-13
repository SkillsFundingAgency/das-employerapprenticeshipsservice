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
using AuthorizationContext = SFA.DAS.Authorization.Context.AuthorizationContext;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers
{
    [TestFixture]
    public class DefaultAuthorizationHandlerTests
    {
        public List<string> Options { get; set; }
        public IAuthorizationContext AuthorizationContext { get; set; }
        public DefaultAuthorizationHandler SutDefaultAuthorizationHandler { get; set; }
        public AuthorizationContextTestsFixture AuthorizationContextTestsFixture { get; set; }
        public const string TeamViewUrl = "accounts/{hashedaccountid}/teams/view";
        public const string HomeUrl = "accounts/{hashedaccountid}/teams";


        [SetUp]
        public void Arrange()
        {
            AuthorizationContextTestsFixture = new AuthorizationContextTestsFixture();
            Options = new List<string>();
            SutDefaultAuthorizationHandler = new DefaultAuthorizationHandler();
            AuthorizationContext = new AuthorizationContext();
        }

        [Test]
        public async Task GetAuthorizationResult_WhenTheUserIsTier2_ThenApply()
        {
            //Act
            var authorizationResult = await SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContext);

            //Assert
            authorizationResult.IsAuthorized.Should().Be(true);
        }

        [Test]
        public void GetAuthorizationResult_AuthorizationContextn()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetTestData(3);

            //Act   
            var result = AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            result.Should().Be("Foo_0: Bar_0, Foo_1: Bar_1, Foo_2: Bar_2");
        }

        [Test]
        public void Test_AuthorizationContext_View_Page()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(TeamViewUrl);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
        }

        [Test]
        public void Test_AuthorizationContext_Home_Page()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(HomeUrl);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(false);
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
        public Mock<IRouteHandler> MockRouteHandler { get; set; }
        public const string Tier2User = "Tier2User";
      


        public AuthorizationContextTestsFixture()
        {
            Key = "Foo";
            Value = "Bar";
            AuthorizationContext = new AuthorizationContext();
            MockContextBase = new Mock<HttpContextBase>();
            MockRequestBase = new Mock<HttpRequestBase>();
            MockResponseBase = new Mock<HttpResponseBase>();
            MockRouteHandler = new Mock<IRouteHandler>();
            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
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


        public AuthorizationContextTestsFixture SetData(string url)
        {              
            var routebase = new Route(url, MockRouteHandler.Object);
            var routeData = new RouteData(routebase, MockRouteHandler.Object);
            AuthorizationContext.Set("RouteData", routeData);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, Tier2User));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
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
