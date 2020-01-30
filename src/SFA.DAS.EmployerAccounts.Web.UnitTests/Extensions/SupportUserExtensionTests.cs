using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class SupportUserExtensionTests
    {
        private Mock<ILogger> _logger;
        private SupportConsoleAuthenticationOptions supportConsoleAuthenticationOptions;
        private TestAppBuilder sut;

        [SetUp]
        public void Arrange()
        {
            //Arrange
            _logger = new Mock<ILogger>();
            sut = new TestAppBuilder();
            supportConsoleAuthenticationOptions = new SupportConsoleAuthenticationOptions
            {
                AdfsOptions = new ADFSOptions
                {
                    MetadataAddress = "https://adfs.preprod.skillsfunding.service.gov.uk/FederationMetadata/2007-06/FederationMetadata.xml",
                    Wreply = "https://localhost:44344",
                    Wtrealm = "https://localhost:44344",
                    BaseUrl = "https://at-login.apprenticeships.education.gov.uk/identity"
                },
                Logger = _logger.Object
            };
        }

        [Test]
        public void GivenSupportUserHasAccess_WhenAuthenticateSupportUser_ThenVerifyCookieAuthentications()
        {
            //Act
            var result = SupportUserExtensions.UseSupportConsoleAuthentication(sut, supportConsoleAuthenticationOptions) as TestAppBuilder;

            //Assert
            Assert.IsNotNull(result);
            foreach (Tuple<object, object[]> middleware in result.BuildMiddleware)
            {
                object[] invokeParameters = InvokeParameters(middleware);
                CookieAuthenticationOptions cookieAuthenticationOptions;
                foreach (var obj in invokeParameters.Where(obj => obj is CookieAuthenticationOptions).Select(obj => obj))
                {
                    cookieAuthenticationOptions = (CookieAuthenticationOptions)obj;
                    Assert.AreEqual(CookieAuthenticationType(cookieAuthenticationOptions.AuthenticationType), cookieAuthenticationOptions.AuthenticationType);
                }
            }
        }

        [Test]
        public void GivenSupportUserHasAccess_WhenAuthenticateSupportUser_ThenVerifyWsFederationAuthenticationOptions()
        {
            //Act
            var result = SupportUserExtensions.UseSupportConsoleAuthentication(sut, supportConsoleAuthenticationOptions) as TestAppBuilder;

            //Assert            
            Assert.IsNotNull(result);
            foreach (Tuple<object, object[]> middleware in result.BuildMiddleware)
            {
                object[] invokeParameters = InvokeParameters(middleware);
                
                WsFederationAuthenticationOptions wsFederationAuthenticationOptions;
                foreach (var obj in invokeParameters.Where(obj => obj is WsFederationAuthenticationOptions).Select(obj => obj))
                {
                    wsFederationAuthenticationOptions = (WsFederationAuthenticationOptions)obj;
                    Assert.AreEqual("Staff", wsFederationAuthenticationOptions.AuthenticationType);
                    Assert.AreEqual(supportConsoleAuthenticationOptions.AdfsOptions.Wtrealm, wsFederationAuthenticationOptions.Wtrealm);
                    Assert.AreEqual(supportConsoleAuthenticationOptions.AdfsOptions.MetadataAddress, wsFederationAuthenticationOptions.MetadataAddress);
                    Assert.AreEqual(supportConsoleAuthenticationOptions.AdfsOptions.Wreply, wsFederationAuthenticationOptions.Wreply);
                    Assert.IsNotNull(wsFederationAuthenticationOptions.Notifications);
                }
            }
        }

        [Test]
        public void GivenSupportUserHasAccess_WhenAuthenticateSupportUser_ThenVerifyMapOptions()
        {
            //Act
            var result = SupportUserExtensions.UseSupportConsoleAuthentication(sut, supportConsoleAuthenticationOptions) as TestAppBuilder;

            //Assert
            Assert.IsNotNull(result);
            foreach (Tuple<object, object[]> middleware in result.BuildMiddleware)
            {
                object[] invokeParameters = InvokeParameters(middleware);

                Microsoft.Owin.Mapping.MapOptions mapOptions;
                foreach (var obj in invokeParameters.Where(obj => obj is Microsoft.Owin.Mapping.MapOptions).Select(obj => obj))
                {
                    mapOptions = obj as Microsoft.Owin.Mapping.MapOptions;
                    Assert.AreEqual(PathMatch(mapOptions.PathMatch.Value), mapOptions.PathMatch.Value);
                }
            }
        }        

        private static object[] InvokeParameters(Tuple<object, object[]> middleware)
        {
            object app = new Func<IDictionary<string, object>, Task>(new NotFound().Invoke);
            object middlewareDelegate = middleware.Item1;
            object[] middlewareArgs = middleware.Item2;
            object[] invokeParameters = new[] { app }.Concat(middlewareArgs).ToArray();
            return invokeParameters;
        }

        
        private static string CookieAuthenticationType(string authType)
        {
            if (authType.Equals("Staff")) { return "Staff";  }
            if (authType.Equals("Employer")) { return "Employer"; }
            return string.Empty;
        }

        private static string PathMatch(string pathString)
        {
            if (pathString.Equals("/login")) { return "/login"; }
            if (pathString.Equals("/login/staff")) { return "/login/staff"; }
            return string.Empty;
        }
        
    }
}
