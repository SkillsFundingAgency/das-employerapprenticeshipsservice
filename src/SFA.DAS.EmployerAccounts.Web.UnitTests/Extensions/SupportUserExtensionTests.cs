using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Security.WsFederation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions;

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
        var result = sut.UseSupportConsoleAuthentication(supportConsoleAuthenticationOptions) as TestAppBuilder;

        //Assert
        var cookieOptions = result.MiddlewareOptions.OfType<CookieAuthenticationOptions>();
        //Assert.IsNotNull(cookieOptions.Single(i => i.AuthenticationType.Equals("Staff")));
        //Assert.IsNotNull(cookieOptions.Single(i => i.AuthenticationType.Equals("Employer")));
        //Assert.IsTrue(cookieOptions.Single(i => i.AuthenticationType.Equals("Staff")).SlidingExpiration);
        //Assert.IsTrue(cookieOptions.Single(i => i.AuthenticationType.Equals("Employer")).SlidingExpiration);
        //Assert.AreEqual(14, cookieOptions.Single(i => i.AuthenticationType.Equals("Staff")).ExpireTimeSpan.Days);
        //Assert.AreEqual(10, cookieOptions.Single(i => i.AuthenticationType.Equals("Employer")).ExpireTimeSpan.Minutes);
    }

    [Test]
    public void GivenSupportUserHasAccess_WhenAuthenticateSupportUser_ThenVerifyWsFederationAuthenticationOptions()
    {
        //Act
        var result = sut.UseSupportConsoleAuthentication(supportConsoleAuthenticationOptions) as TestAppBuilder;

        //Assert                        
        var wsFederationAuthenticationOptions = result.MiddlewareOptions.OfType<WsFederationAuthenticationOptions>().Single();
        Assert.AreEqual("Staff", wsFederationAuthenticationOptions.AuthenticationType);
        Assert.AreEqual(supportConsoleAuthenticationOptions.AdfsOptions.Wtrealm, wsFederationAuthenticationOptions.Wtrealm);
        Assert.AreEqual(supportConsoleAuthenticationOptions.AdfsOptions.MetadataAddress, wsFederationAuthenticationOptions.MetadataAddress);
        Assert.AreEqual(supportConsoleAuthenticationOptions.AdfsOptions.Wreply, wsFederationAuthenticationOptions.Wreply);
        Assert.IsNotNull(wsFederationAuthenticationOptions.Notifications);
            
    }

    [Test]
    public void GivenSupportUserHasAccess_WhenAuthenticateSupportUser_ThenVerifyMapOptions()
    {
        //Act
        var result = sut.UseSupportConsoleAuthentication(supportConsoleAuthenticationOptions) as TestAppBuilder;

        //Assert
        var mapOptions = result.MiddlewareOptions.OfType<Microsoft.Owin.Mapping.MapOptions>();
        Assert.IsNotNull(mapOptions.Select(i => i.PathMatch.Equals("/login")));
        Assert.IsNotNull(mapOptions.Select(i => i.PathMatch.Equals("/login/staff")));            
    }
}