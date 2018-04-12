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

        [When(@"I send a levy transfer to a company")]
        public void WhenISendALevyTransferToACompany()
        {
            var senderAccountId = (long)ScenarioContext.Current["AccountId"];
            var receiverAccountId = senderAccountId + 1;

            var transactionRespository = _container.GetInstance<ITransactionRepository>();

            _transferTransactions.Add(new TransferTransactionLine
            {
                AccountId = senderAccountId,
                Amount = 300.605M,
                SenderAccountId = senderAccountId,
                SenderAccountName = "My Company Inc",
                ReceiverAccountId = receiverAccountId,
                ReceiverAccountName = "Tester Corp",
                DateCreated = DateTime.Now,
                TransactionType = TransactionItemType.Transfer,
                TransactionDate = DateTime.Now,
                PeriodEnd = "1718-R01"
            });


            transactionRespository.CreateTransferTransactions(_transferTransactions);
        }

        [When(@"I receive a levy transfer from a company")]
        public void WhenIReceiveALevyTransferFromACompany()
        {
            var receiverAccountId = (long)ScenarioContext.Current["AccountId"];
            var senderAccountId = receiverAccountId + 1;

            var transactionRespository = _container.GetInstance<ITransactionRepository>();

            _transferTransactions.Add(new TransferTransactionLine
            {
                AccountId = receiverAccountId,
                Amount = 300.605M,
                SenderAccountId = senderAccountId,
                SenderAccountName = "My Company Inc",
                ReceiverAccountId = receiverAccountId,
                ReceiverAccountName = "Tester Corp",
                DateCreated = DateTime.Now,
                TransactionType = TransactionItemType.Transfer,
                TransactionDate = DateTime.Now,
                PeriodEnd = "1718-R01"
            });


            transactionRespository.CreateTransferTransactions(_transferTransactions);
        }

        [Then(@"I should see a transfer sent transaction on the transaction summary page")]
        public void ThenIShouldSeeATransferSentTransactionOnTheTransactionSummaryPage()
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

        [Then(@"I should see a transfer received transaction on the transaction summary page")]
        public void ThenIShouldSeeATransferReceivedTransactionOnTheTransactionSummaryPage()
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();

            var actual = employerAccountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, DateTime.Now.Year, DateTime.Now.Month, userId).Result;

            var actualTransaction = actual.Data.Model.Data.TransactionLines.OfType<TransferTransactionLine>().Single();

            var expectedTransaction = _transferTransactions.Single();

            Assert.AreEqual(expectedTransaction.DateCreated.ToShortDateString(), actualTransaction.DateCreated.ToShortDateString());
            Assert.AreEqual(expectedTransaction.Amount, actualTransaction.Amount);
            Assert.AreEqual($"Transfer received from {expectedTransaction.SenderAccountName}", actualTransaction.Description);
        }

    }
}
