using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.TransferDashboardSteps
{
    [Binding]
    public class TransferDashboardSteps : TechTalk.SpecFlow.Steps
    {

        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private string _hashedAccountId;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private static Mock<IEmployerCommitmentApi> _commitmentsApi;
        private static LevyDeclarationProviderConfiguration _levyDeclarationProviderConfiguration;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _levyDeclarationProviderConfiguration =
                ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>(IoC.LevyAggregationProviderName);

            _container = IoC.CreateContainer(
                _messagePublisher,
                _owinWrapper,
                _cookieService,
                _eventsApi,
                _commitmentsApi,
                _levyDeclarationProviderConfiguration);
        }

        [AfterFeature]
        public static void TearDown()
        {
            _container.Dispose();
        }

        [When(@"The transfer allowance ratio is (.*) percent")]
        public void WhenTheTransferAllowanceRatioIsPercent(int percentage)
        {
            _levyDeclarationProviderConfiguration.TransferAllowanceRatio = percentage / 100f;
        }


        [Then(@"the transfer allowance should be (.*) on the transfer dashboard screen")]
        public void ThenTheTransferAllowanceShouldBeOnTheTransferDashboardScreen(decimal expectedTransferBalance)
        {
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserId"].ToString();

            var orchestrator = _container.GetInstance<TransferOrchestrator>();
            var response = orchestrator.GetTransferAllowance(hashedAccountId, userId).Result;

            Assert.IsNotNull(response?.Data);

            Assert.AreEqual(expectedTransferBalance.ToString("C0"), response.Data.TransferAllowance.ToString("C0"));
        }
    }
}


