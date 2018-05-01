using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Commitment;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
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
            var owinWrapper = new Mock<IAuthenticationService>();
            var cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            var eventsApi = new Mock<IEventsApi>();
            var commitmentsApi = new Mock<IEmployerCommitmentApi>();
            

            _container = IoC.CreateContainer(messagePublisher, owinWrapper, cookieService, eventsApi, commitmentsApi);

            var commitmentService = new Mock<ICommitmentService>();
            commitmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>())).ReturnsAsync(new List<Cohort>());
            _container.Inject(commitmentService.Object);
        }

        [When(@"I sign Agreement")]
        public void WhenISignAgreement()
        {
            
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerAgreementOrchestrator = _container.GetInstance<EmployerAgreementOrchestrator>();

            var agreement = employerAgreementOrchestrator.Get(hashedId, userId).Result.Data.EmployerAgreementsData.EmployerAgreements.FirstOrDefault();

            employerAgreementOrchestrator.SignAgreement(agreement.Pending.HashedAgreementId, hashedId, userId, DateTime.Today,"company name").Wait();

        }

        [Then(@"Agreement Status is ""(.*)""")]
        public void ThenAgreementStatusIs(string status)
        {
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerAgreementOrchestrator = _container.GetInstance<EmployerAgreementOrchestrator>();
            var agreement = employerAgreementOrchestrator.Get(hashedId, userId).Result.Data.EmployerAgreementsData.EmployerAgreements.FirstOrDefault();

            Assert.IsNotNull(agreement);
            Assert.IsTrue(agreement.HasSignedAgreement);
        }
    }
}
