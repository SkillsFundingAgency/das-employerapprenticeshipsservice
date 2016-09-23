using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewThePrivacyPage
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _controller;
        private Mock<HomeOrchestrator> _orchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _whiteList;

        [SetUp]
        public void Init()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _orchestrator = new Mock<HomeOrchestrator>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();
            _featureToggle = new Mock<IFeatureToggle>();
            _whiteList = new Mock<IUserWhiteList>();

            _controller = new HomeController(
                _owinWrapper.Object, _orchestrator.Object, _configuration, _featureToggle.Object, _whiteList.Object);
        }

        [Test]
        public void ThenIShouldSeeTheCorrectLinks()
        {
            //Assign
            _configuration.Privacy = new PrivacyConfiguration
            {
                AboutCookiesUrl = "about",
                ApplicationInsightsUrl = "application insights",
                GoogleAnalyticsUrl = "google",
                SurveyProviderUrl = "Survey"
            };

            //Act
            var result = _controller.Privacy() as ViewResult;

            //Assert
            Assert.IsNotNull(result);

            var model = result.Model as PrivacyViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(_configuration.Privacy.AboutCookiesUrl, model.AboutUrl);
            Assert.AreEqual(_configuration.Privacy.ApplicationInsightsUrl, model.ApplicationInsightsUrl);
            Assert.AreEqual(_configuration.Privacy.GoogleAnalyticsUrl, model.GoogleUrl);
            Assert.AreEqual(_configuration.Privacy.SurveyProviderUrl, model.SurveyProviderUrl);
        }
    }
}
