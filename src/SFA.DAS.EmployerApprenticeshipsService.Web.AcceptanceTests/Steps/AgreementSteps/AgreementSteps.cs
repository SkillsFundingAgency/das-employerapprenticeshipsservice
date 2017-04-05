using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.AgreementSteps
{
    [Binding]
    public class AgreementSteps
    {
        private static IContainer _container;
       
        [BeforeFeature()]
        public static void Arrange()
        {
            var messagePublisher = new Mock<IMessagePublisher>();
            var owinWrapper = new Mock<IOwinWrapper>();
            var cookieService = new Mock<ICookieService<EmployerAccountData>>();
            var eventsApi = new Mock<IEventsApi>();

            _container = IoC.CreateContainer(messagePublisher, owinWrapper, cookieService, eventsApi);
        }

        [When(@"I sign Agreement")]
        public void WhenISignAgreement()
        {
            
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerAgreementOrchestrator = _container.GetInstance<EmployerAgreementOrchestrator>();

            var agreement = employerAgreementOrchestrator.Get(hashedId, userId).Result.Data.EmployerAgreements.FirstOrDefault();

            employerAgreementOrchestrator.SignAgreement(agreement.HashedAgreementId, hashedId, userId, DateTime.Today).Wait();

        }

        [Then(@"Agreement Status is ""(.*)""")]
        public void ThenAgreementStatusIs(string status)
        {
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerAgreementOrchestrator = _container.GetInstance<EmployerAgreementOrchestrator>();
            var agreement = employerAgreementOrchestrator.Get(hashedId, userId).Result.Data.EmployerAgreements.FirstOrDefault();

            Assert.IsNotNull(agreement);
            Assert.AreEqual(status,agreement.Status.ToString());
        }


    }
}
