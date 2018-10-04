using System;
using System.Linq;
using System.Threading.Tasks;
using BoDi;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class FinanceSteps : TechTalk.SpecFlow.Steps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public FinanceSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Then(@"we should see a level 1 screen with a balance of (.*) on the (.*)/(.*)")]
        public async Task ThenLevel1HasRowWithCorrectBalance(int balance, int month, int year)
        {
            var account = _objectContext.Get<Account>();

            var actual = await _objectContainer.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

            Assert.AreEqual(balance, actual.Data.Model.CurrentBalance);
        }

        [Then(@"we should see a level 1 screen with a total levy of (.*) on the (.*)/(.*)")]
        public async Task ThenLevel1HasRowWithCorrectTotalLevy(int totalLevy, int month, int year)
        {
            var account = _objectContext.Get<Account>();

            var actual = await _objectContainer.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

            Assert.AreEqual(totalLevy, actual.Data.Model.Data.TransactionLines.Sum(t => t.Amount));
        }

        [Then(@"we should see a level 2 screen with a levy declared of ([^ ]*) on the (.*)/(.*)")]
        public async Task ThenUserDaveFromAccountAShouldSeeALevelScreenWithALevyDeclaredOfOnThe(int levyDeclared, int month, int year)
        {
            var account = _objectContext.Get<Account>();

            var fromDate = new DateTime(year, month, 1);
            var toDate = new DateTime(year, month + 1, 1).AddMilliseconds(-1);

            var viewModel = await _objectContainer.Resolve<EmployerAccountTransactionsOrchestrator>().FindAccountLevyDeclarationTransactions(account.HashedId, fromDate, toDate, "userRef");

            Assert.AreEqual(levyDeclared, viewModel.Data.Amount - viewModel.Data.SubTransactions.Sum(x => x.TopUp));
        }

        [Then(@"we should see a level 2 screen with a top up of ([^ ]*) on the (.*)/(.*)")]
        public async Task ThenUserDaveFromAccountAShouldSeeALevelScreenWithATopUpOfOnThe(int topUp, int month, int year)
        {
            var account = _objectContext.Get<Account>();

            var fromDate = new DateTime(year, month, 1);
            var toDate = new DateTime(year, month + 1, 1).AddMilliseconds(-1);

            var viewModel = await _objectContainer.Resolve<EmployerAccountTransactionsOrchestrator>().FindAccountLevyDeclarationTransactions(account.HashedId, fromDate, toDate, "userRef");

            var topUpTotal = viewModel.Data.SubTransactions.Sum(x => x.TopUp);

            Assert.AreEqual(topUp, topUpTotal);
        }
    }
}