using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.FinancialDashboardSteps
{
    [Binding]
    public class FinancialDashboardSteps : TechTalk.SpecFlow.Steps
    {

        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private string _hashedAccountId;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private static Mock<IEmployerCommitmentApi> _commitmentsApi;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);
        }

        [AfterFeature]
        public static void TearDown()
        {
            _container.Dispose();
        }

        [Then(@"the transfer balance should be (.*) on the financial dashboard screen")]
        public void ThenTheTransferBalanceShouldBeOnTheFinancialDashboardScreen(decimal expectedTransferBalance)
        {
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserId"].ToString();

            var orchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var response = orchestrator.GetFinanceDashboardViewModel(hashedAccountId, 0, 0, userId).Result;

            Assert.IsNotNull(response?.Data);

            Assert.AreEqual(expectedTransferBalance, response.Data.CurrentTransferFunds);
        }
    }
}


