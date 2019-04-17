using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class FinanceSteps : TechTalk.SpecFlow.Steps
    {
        public const int StepTimeout = 2 * 60 * 1000;

        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public FinanceSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given("the account has transactions")]
        public Task GivenTheAccountHasTransactions(Table table)
        {
            return _objectContainer.ScopeAsync(c =>
            {
                var account = _objectContext.Get<Account>();
                var empRef = _objectContext.GetEmpRef();
                var submissionId = 999000101;

                var transactionLines = table.Rows.Select(r => new TransactionLineEntity
                {
                    AccountId = account.Id,
                    TransactionType = (TransactionItemType)Enum.Parse(typeof(TransactionItemType), r["TransactionType"]),
                    TransactionDate = DateTime.UtcNow.Date,
                    DateCreated = DateTime.Parse(r["DateCreated"]),
                    Amount = decimal.Parse(r["Amount"]),
                    EmpRef = empRef,
                    SubmissionId = submissionId++
                });

                var testTransactionRepository = c.Resolve<ITestTransactionRepository>();

                return testTransactionRepository.CreateTransactionLines(transactionLines);
            });
        }
        
        [Given("the account has transactions")]
        public Task GivenTheAccountHasTransactions(Table table)
        {
            return _objectContainer.ScopeAsync(c =>
            {
                var account = _objectContext.Get<Account>();
                var empRef = _objectContext.GetEmpRef();
                var submissionId = 999000101;

                var transactionLines = (
                    from r in table.Rows
                    let t = (TransactionItemType)Enum.Parse(typeof(TransactionItemType), r["TransactionType"])
                    let d = DateTime.Parse(r["DateCreated"])
                    select new TransactionLineEntity
                    {
                        AccountId = account.Id,
                        TransactionType = t,
                        DateCreated = d,
                        TransactionDate = d,
                        Amount = decimal.Parse(r["Amount"]),
                        EmpRef = t == TransactionItemType.Declaration ? empRef : null,
                        SubmissionId = t == TransactionItemType.Declaration ? submissionId++ : (int?)null,
                        PeriodEnd = r.ContainsKey("PeriodEnd") ? r["PeriodEnd"] : null
                    })
                    .ToList();

                var testTransactionRepository = c.Resolve<ITestTransactionRepository>();

                return testTransactionRepository.CreateTransactionLines(transactionLines);
            });
        }

        [When("the expire funds process runs on (.*) with a (.*) month expiry period")]
        public async Task WhenTheExpireFundsProcessRuns(DateTime date, int expiryPeriod)
        {
            var configuration = _objectContainer.Resolve<EmployerFinanceConfiguration>();
            var currentDateTime = _objectContainer.Resolve<Mock<ICurrentDateTime>>();
            var account = _objectContext.Get<Account>();
            var messageSession = _objectContainer.Resolve<IMessageSession>();

            currentDateTime.Setup(c => c.Now).Returns(date);
            configuration.FundsExpiryPeriod = expiryPeriod;

            await messageSession.Send(new ExpireAccountFundsCommand { AccountId = account.Id });

            await _objectContainer.ScopeAsync(async c =>
            {
                var transactionRepository = c.Resolve<ITransactionRepository>();
                var timeout = Debugger.IsAttached ? 10 * 60 * 1000 : StepTimeout;
                var cancellationTokenSource = new CancellationTokenSource(timeout);
                var isComplete = await transactionRepository.WaitForAllTransactionLinesInDatabase(account, cancellationTokenSource.Token);

                if (!isComplete)
                {
                    throw new Exception($"The transactions have not been completely loaded within the allowed time ({timeout} msecs). Either they are still loading or something has failed.");
                }
            });
        }


            await messageSession.Send(new ExpireAccountFundsCommand { AccountId = account.Id });

            await _objectContainer.ScopeAsync(async c =>
            {
                var transactionRepository = c.Resolve<ITransactionRepository>();
                var timeout = Debugger.IsAttached ? 10 * 60 * 1000 : StepTimeout;
                var cancellationTokenSource = new CancellationTokenSource(timeout);
                var isComplete = await transactionRepository.WaitForAllTransactionLinesInDatabase(account, cancellationTokenSource.Token);

                if (!isComplete)
                {
                    Assert.Fail($"The transactions have not been completely loaded within the allowed time ({timeout} msecs). Either they are still loading or something has failed.");
                }
            });
        }

        [Then(@"we should see a level 1 screen with a balance of (.*) on the (.*)/(.*)")]
        public Task ThenLevel1HasRowWithCorrectBalance(int balance, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var actual = await c.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

                Assert.AreEqual(balance, actual.Data.Model.CurrentBalance);
            });
        }

        [Then(@"we should see a level 1 screen with a total levy of (.*) on the (.*)/(.*)")]
        public Task ThenLevel1HasRowWithCorrectTotalLevy(int totalLevy, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var actual = await c.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

                Assert.AreEqual(totalLevy, actual.Data.Model.Data.TransactionLines.Where(t => t.TransactionType == TransactionItemType.Declaration).Sum(t => t.Amount));
            });
        }

        [Then(@"we should see a level 1 screen with a total payment of (.*) on the (.*)/(.*)")]
        public Task ThenLevel1HasRowWithCorrectTotalPayment(int totalPayment, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var actual = await c.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

                Assert.AreEqual(totalPayment, actual.Data.Model.Data.TransactionLines.Where(t => t.TransactionType == TransactionItemType.Payment).Sum(t => t.Amount));
            });
        }

        [Then(@"we should see a level 1 screen with a total transfer of (.*) on the (.*)/(.*)")]
        public Task ThenLevel1HasRowWithCorrectTotalTransfer(int totalTransfer, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var actual = await c.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

                Assert.AreEqual(totalTransfer, actual.Data.Model.Data.TransactionLines.Where(t => t.TransactionType == TransactionItemType.Transfer).Sum(t => t.Amount));
            });
        }

        [Then(@"we should see a level 1 screen with a levy declared of (.*) on the (.*)/(.*)")]
        public Task ThenLevel1HasRowWithCorrectLevyTransaction(int levyDeclared, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var actual = await c.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

                Assert.AreEqual(levyDeclared, actual.Data.Model.Data.TransactionLines.Sum(t => t.Amount));
            });
        }

        [Then(@"we should see a level 2 screen with a levy declared of ([^ ]*) on the (.*)/(.*)")]
        public Task ThenUserDaveFromAccountAShouldSeeALevelScreenWithALevyDeclaredOfOnThe(int levyDeclared, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var fromDate = new DateTime(year, month, 1);
                var toDate = new DateTime(year, month + 1, 1).AddMilliseconds(-1);
                var viewModel = await c.Resolve<EmployerAccountTransactionsOrchestrator>().FindAccountLevyDeclarationTransactions(account.HashedId, fromDate, toDate, "userRef");

                Assert.AreEqual(levyDeclared, viewModel.Data.Amount - viewModel.Data.SubTransactions.Sum(x => x.TopUp));
            });
        }

        [Then("we should see a level 1 screen with expired levy of (.*) on the (.*)/(.*)")]
        public Task ThenLevel1HasRowWithCorrectExpiredLevy(int expiredLevy, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var actual = await c.Resolve<EmployerAccountTransactionsOrchestrator>().GetAccountTransactions(account.HashedId, year, month, "userRef");

                Assert.AreEqual(expiredLevy, actual.Data.Model.Data.TransactionLines.Where(t => t.TransactionType == TransactionItemType.ExpiredFund).Sum(t => t.Amount));
            });
        }

        [Then(@"we should see a level 2 screen with a top up of ([^ ]*) on the (.*)/(.*)")]
        public Task ThenUserDaveFromAccountAShouldSeeALevelScreenWithATopUpOfOnThe(int topUp, int month, int year)
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var account = _objectContext.Get<Account>();
                var fromDate = new DateTime(year, month, 1);
                var toDate = new DateTime(year, month + 1, 1).AddMilliseconds(-1);
                var viewModel = await c.Resolve<EmployerAccountTransactionsOrchestrator>().FindAccountLevyDeclarationTransactions(account.HashedId, fromDate, toDate, "userRef");
                var topUpTotal = viewModel.Data.SubTransactions.Sum(x => x.TopUp);

                Assert.AreEqual(topUp, topUpTotal);
            });
        }
    }
}