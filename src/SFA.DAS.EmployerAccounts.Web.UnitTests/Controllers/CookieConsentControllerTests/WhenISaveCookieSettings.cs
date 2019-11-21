using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.CookieConsentControllerTests
{
    public class WhenISaveCookieSettings : ControllerTestBase
    {
        private CookieConsentController _cookieConsentController;
        private HttpCookieCollection _cookieCollection;
        private bool _expectedAnalyticsConsent;
        private bool _expectedMarketingConsent;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();
            _expectedAnalyticsConsent = true;
            _expectedMarketingConsent = true;
            _cookieConsentController = new CookieConsentController(Mock.Of<IAuthenticationService>(),
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>());
            _cookieConsentController.ControllerContext = _controllerContext.Object;
            _cookieCollection = new HttpCookieCollection();
            _httpResponse.SetupGet(x => x.Cookies).Returns(_cookieCollection);
        }

        [Test]
        public void ThenTheViewIsReturnedWithThePassedInValueForSaved()
        {
            var result = _cookieConsentController.Settings(_expectedAnalyticsConsent, _expectedMarketingConsent);
            Assert.That(result, Is.Not.Null);
            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.That(redirectToRouteResult, Is.Not.Null);
            Assert.That(redirectToRouteResult.RouteValues["action"],  Is.EqualTo("Settings"));
            Assert.That(redirectToRouteResult.RouteValues["saved"], Is.True);
        }

        [Test]
        public void ThenTheAnalyticsConsentCookieIsSet()
        {
            _cookieConsentController.Settings(_expectedAnalyticsConsent, _expectedMarketingConsent);
            Assert.That(_cookieCollection.Get("AnalyticsConsent"), Is.Not.Null);
            Assert.That(_cookieCollection.Get("AnalyticsConsent").Value, Is.EqualTo(_expectedAnalyticsConsent.ToString().ToLower()));
        }

        [Test]
        public void ThenTheMarketingConsentCookieIsSet()
        {
            _cookieConsentController.Settings(_expectedAnalyticsConsent, _expectedMarketingConsent);
            Assert.That(_cookieCollection.Get("MarketingConsent"), Is.Not.Null);
            Assert.That(_cookieCollection.Get("MarketingConsent").Value, Is.EqualTo(_expectedAnalyticsConsent.ToString().ToLower()));
        }

        [Test]
        public void ThenTheSeenCookieMessageCookieIsSet()
        {
            _cookieConsentController.Settings(_expectedAnalyticsConsent, _expectedMarketingConsent);
            Assert.That(_cookieCollection.Get("DAS-SeenCookieMessage"), Is.Not.Null);
            Assert.That(_cookieCollection.Get("DAS-SeenCookieMessage").Value, Is.EqualTo(true.ToString().ToLower()));
        }

        [Test]
        public void ThenTheCookieConsentMessageCookieIsSet()
        {
            _cookieConsentController.Settings(_expectedAnalyticsConsent, _expectedMarketingConsent);
            Assert.That(_cookieCollection.Get("CookieConsent"), Is.Not.Null);
            Assert.That(_cookieCollection.Get("CookieConsent").Value, Is.EqualTo(true.ToString().ToLower()));
        }
    }
}