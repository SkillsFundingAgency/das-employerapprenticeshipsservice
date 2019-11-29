using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.CookieConsentControllerTests
{
    public class WhenIViewTheSettingsPage : ControllerTestBase
    {
        private CookieConsentController _cookieConsentController;
        private bool _expectedSaved;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();
            _expectedSaved = true;
            _cookieConsentController = new CookieConsentController(Mock.Of<IAuthenticationService>(),
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>());
        }

        [Test]
        public void ThenTheViewIsReturnedWithThePassedInValueForSaved()
        {
            var result = _cookieConsentController.Settings(_expectedSaved);
            Assert.That(result, Is.Not.Null);
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.AreEqual(viewResult.Model.GetType().GetProperty("Saved").GetValue(viewResult.Model), _expectedSaved);
        }
    }
}
