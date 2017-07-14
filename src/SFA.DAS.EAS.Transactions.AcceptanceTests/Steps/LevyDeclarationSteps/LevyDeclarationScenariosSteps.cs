using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.LevyDeclarationSteps
{
    [Binding]
    public class LevyDeclarationScenariosSteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
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

        [Then(@"the total levy shown for month (.*)/(.*) should be (.*)")]
        public void ThenTheLevyShownForMonthShouldBe(int month, int year, int levyAmount)
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserId"].ToString();

            var actual = employerAccountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, year, month, userId).Result;

            var monthTransaction = actual.Data.Model.Data.TransactionLines.Single();

            Assert.AreEqual(levyAmount, monthTransaction.Amount);
        }

        [Then(@"the balance on (.*)/(.*) should be (.*) on the screen")]
        public void ThenTheBalanceOnShouldBeOnTheScreen(int month, int year, int balance)
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserId"].ToString();

            var actual = employerAccountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, year, month, userId).Result;

            Assert.AreEqual(balance, actual.Data.Model.CurrentBalance);
        }

        [Then(@"For month (.*)/(.*) the levy declared should be (.*) and the topup should be (.*)")]
        public void ThenTheLevyTopupForMonthShouldBe(int month, int year, int levyDeclared, int topup)
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserId"].ToString();

            var actual = employerAccountTransactionsOrchestrator.FindAccountLevyDeclarationTransactions(hashedAccountId,
                new DateTime(year, month, 1), new DateTime(year, month, 30), userId).Result;

            var monthTransaction = actual.Data.SubTransactions.First();

            Assert.AreEqual(topup, monthTransaction.TopUp);
        }
    }
}
