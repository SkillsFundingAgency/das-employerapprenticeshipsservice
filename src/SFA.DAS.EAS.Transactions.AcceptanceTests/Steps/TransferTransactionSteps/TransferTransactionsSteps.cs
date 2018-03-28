using NUnit.Framework;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.CommonSteps;
using SFA.DAS.EAS.Web.Orchestrators;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.TransferTransactionSteps
{
    [Binding]
    public class TransferTransactionsSteps
    {
        private readonly IContainer _container;
        private ICollection<TransferTransactionLine> _transferTransactions;

        public TransferTransactionsSteps()
        {
            _container = new AccountCreationSteps().Container;
        }

        [BeforeScenario]
        public void Setup()
        {
            _transferTransactions = new List<TransferTransactionLine>();
        }

        [When(@"I add transfer transactions")]
        public void WhenIAddTransferTransactions()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];

            var transactionRespository = _container.GetInstance<ITransactionRepository>();

            _transferTransactions.Add(new TransferTransactionLine
            {
                AccountId = accountId,
                Amount = 300.605M,
                ReceiverAccountName = "Tester Corp",
                DateCreated = DateTime.Now,
                TransactionType = TransactionItemType.Transfer,
                TransactionDate = DateTime.Now
            });


            transactionRespository.CreateTransferTransactions(_transferTransactions);
        }

        [Then(@"They should appear in the transaction summary page")]
        public void ThenTheyShouldAppearInTheTransactionSummaryPage()
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();

            var actual = employerAccountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, DateTime.Now.Year, DateTime.Now.Month, userId).Result;

            var actualTransaction = actual.Data.Model.Data.TransactionLines.OfType<TransferTransactionLine>().Single();

            var expectedTransaction = _transferTransactions.Single();

            Assert.AreEqual(expectedTransaction.DateCreated.ToShortDateString(), actualTransaction.DateCreated.ToShortDateString());
            Assert.AreEqual(expectedTransaction.Amount, actualTransaction.Amount);
            Assert.AreEqual($"Transfer sent to {expectedTransaction.ReceiverAccountName}", actualTransaction.Description);
        }
    }
}
