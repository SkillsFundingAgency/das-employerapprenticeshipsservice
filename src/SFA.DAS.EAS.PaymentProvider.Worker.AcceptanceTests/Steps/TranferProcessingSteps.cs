using Microsoft.ServiceBus.Messaging;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.ApprenticeshipInfoService;
using SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.PaymentService;
using SFA.DAS.EAS.TestCommon.DbCleanup;
using SFA.DAS.EAS.TestCommon.TestModels;
using SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.CommonSteps;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using ITransferRepository = SFA.DAS.EAS.Domain.Data.Repositories.ITransferRepository;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;
using PeriodEnd = SFA.DAS.Provider.Events.Api.Types.PeriodEnd;
using Transfer = SFA.DAS.Provider.Events.Api.Types.Transfer;

namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.Steps
{
    [Binding]
    public class TranferProcessingSteps
    {
        private const long ApprenticeshipId = 3;
        private const decimal TransferPaymentAmount = 321.5823M;
        private const string CourseName = "Trainging";

        private readonly AccountCreationSteps _accountCreationSteps;
        private PaymentsProviderWorkerRoleTestWrapper _worker;
        private PaymentServiceApiMessageHandler _paymentServiceApiHandler;
        private PaymentServiceApi _paymentServiceApi;
        private readonly NLogLogger _logger;
        private IMessagePublisher _messagePublisher;
        private PeriodEnd _periodEnd;
        private string _messageBusConnectionString;
        private Task _workerTask;
        private List<Payment> _payments;
        private List<Transfer> _transfers;
        private List<StandardSummary> _standards;
        private ApprenticeshipInfoServiceApi _apprenticeshipInfoServiceApi;
        private ApprenticeshipInfoServiceApiMessageHandler _apprenticeshipInfoServiceApiHandler;

        public TranferProcessingSteps()
        {
            _accountCreationSteps = new AccountCreationSteps();
            _logger = new NLogLogger();
        }

        [BeforeScenario]
        public void SetUp()
        {
            var lapConfig = ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider");

            new CleanTransactionsDatabase(lapConfig, _logger).Execute().Wait();

            _logger.Debug("Getting EAS Config");
            var easConfig = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService");

            _messageBusConnectionString = easConfig.MessageServiceBusConnectionString;

            _logger.Debug("Clearing Payments Completed Queue");

            ClearSubscriptionMessageQueue(
                _messageBusConnectionString,
                MessageGroupHelper.GetMessageGroupName<AccountPaymentsProcessingCompletedMessage>(),
                "MA_TransferDataProcessor_TEST");

            _logger.Debug("Clearing Transfers Completed Queue");

            ClearSubscriptionMessageQueue(
                _messageBusConnectionString,
                MessageGroupHelper.GetMessageGroupName<AccountTransfersProcessingCompletedMessage>(),
                "MA_TransferTransactionProcessor_TEST");

            ClearSubscriptionMessageQueue(
                _messageBusConnectionString,
                MessageGroupHelper.GetMessageGroupName<PaymentProcessorQueueMessage>(),
                "MA_PaymentDataProcessor");

            _logger.Debug("Getting Payments API config");

            var paymentApiClientConfig = ConfigurationHelper.GetConfiguration<PaymentsApiClientConfiguration>("SFA.DAS.PaymentsAPI");

            _logger.Debug("Creating payments API service");

            _paymentServiceApiHandler = new PaymentServiceApiMessageHandler(paymentApiClientConfig.ApiBaseUrl);
            _paymentServiceApi = new PaymentServiceApi(_paymentServiceApiHandler);

            _apprenticeshipInfoServiceApiHandler = new ApprenticeshipInfoServiceApiMessageHandler(easConfig.ApprenticeshipInfoService.BaseUrl);
            _apprenticeshipInfoServiceApi = new ApprenticeshipInfoServiceApi(_apprenticeshipInfoServiceApiHandler);

            _logger.Debug("Starting worker");

            _worker = new PaymentsProviderWorkerRoleTestWrapper();
            _worker.OnStart();

            _workerTask = Task.Run(() => _worker.Run());

            _periodEnd = new PeriodEnd
            {
                Id = "1718-R01",
                CalendarPeriod = new CalendarPeriod { Month = 1, Year = 2018 },
                CompletionDateTime = DateTime.Now.AddDays(2),
                Links = new PeriodEndLinks(),
                ReferenceData = new ReferenceDataDetails()
            };

            _messagePublisher = new TopicMessagePublisher(_messageBusConnectionString, _logger);
        }

        [AfterScenario]
        public void CleanUp()
        {
            _worker.OnStop();

            _apprenticeshipInfoServiceApi?.Dispose();
            _apprenticeshipInfoServiceApiHandler?.Dispose();

            _paymentServiceApi?.Dispose();
            _paymentServiceApiHandler?.Dispose();

            _workerTask.Wait(2000);

            _workerTask.Dispose();
        }

        [Given(@"I have an account")]
        public void GivenIHaveAnAccount()
        {
            _accountCreationSteps.GivenIHaveAnAccount();
        }

        [Given(@"the transfer reciever has an account")]
        public void GivenTheTransferRecieverHasAnAccount()
        {
            var user = _accountCreationSteps.CreateAccount();
            var account = user.Accounts.Single();

            ScenarioContext.Current["RecieverAccount"] = account;
        }

        [Given(@"I get payments for that account")]
        public void GivenIGetPaymentsForThatAccount()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];

            var standardCode = 213;

            _payments = new List<Payment>
            {
                new Payment
                {
                    EmployerAccountId = accountId.ToString(),
                    ApprenticeshipId = ApprenticeshipId,
                    FundingSource = FundingSource.Levy,
                    Amount = TransferPaymentAmount,
                    TransactionType = TransactionType.Learning,
                    Uln = 23687,
                    Ukprn = 738645786,
                    StandardCode = standardCode,
                    EvidenceSubmittedOn = DateTime.Now.AddDays(-3),
                    EmployerAccountVersion = "1.0",
                    ApprenticeshipVersion = "1.0",
                    Id = Guid.NewGuid().ToString(),
                    FrameworkCode = 0,
                    PathwayCode = 0,
                    ProgrammeType = 0,
                    CollectionPeriod = new NamedCalendarPeriod{Id = "1", Month = 1,Year = 2018},
                    DeliveryPeriod = new CalendarPeriod{Month = 1, Year = 2018},
                    ContractType = ContractType.ContractWithEmployer,
                    EarningDetails = new List<Earning>()
                }
            };


            _standards = new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = standardCode.ToString(),
                    Level = 1,
                    Duration = 10,
                    Title = CourseName
                }
            };

            _paymentServiceApiHandler.EmployerAccountId = accountId;
            _paymentServiceApiHandler.PeriodEnd = _periodEnd.Id;

            _apprenticeshipInfoServiceApiHandler.StandardCode = standardCode.ToString();
            _apprenticeshipInfoServiceApiHandler.SetStandards(HttpStatusCode.OK, _standards);

            _paymentServiceApiHandler.SetPeriodEnds(HttpStatusCode.OK, new List<PeriodEnd> { _periodEnd });

            var paymentResults = new PageOfResults<Payment>
            {
                Items = _payments.ToArray(),
                PageNumber = 1,
                TotalNumberOfPages = 1
            };

            _paymentServiceApiHandler.SetPayments(HttpStatusCode.OK, paymentResults);

            _messagePublisher.PublishAsync(new PaymentProcessorQueueMessage
            {
                AccountId = accountId,
                PeriodEndId = _periodEnd.Id,
                AccountPaymentUrl = "test/123"
            }).Wait();
        }

        [Given(@"transfers have been associated with those payments")]
        public void GivenTransfersHaveBeenAssociatedWithThosePayments()
        {
            var senderAccountId = (long)ScenarioContext.Current["AccountId"];
            var receiverAccount = (TestAccount)ScenarioContext.Current["RecieverAccount"];

            _transfers = new List<Transfer>
            {
                new Transfer
                {
                    SenderAccountId = senderAccountId,
                    ReceiverAccountId = receiverAccount.Id,
                    CommitmentId = ApprenticeshipId,
                    Amount = TransferPaymentAmount,
                    Type = TransferType.None,
                    TransferDate = DateTime.Now
                }
            };

            _paymentServiceApiHandler.SetTransfers(HttpStatusCode.OK,
                new PageOfResults<Transfer>
                {
                    Items = _transfers.ToArray(),
                    PageNumber = 1,
                    TotalNumberOfPages = 1
                });
        }

        [Given(@"I have completed getting my payments for that account")]
        [When(@"I finish processing those payments")]
        public void GivenIHaveCompletedGettingMyPaymentsForThatAccount()
        {
            WaitForPaymentsProcessingToComplete();
        }

        [Given(@"I have finished getting transfers for that account")]
        [When(@"I finish processing the transfers for that account")]
        [When(@"I process tranfers transactions for that account")]
        public void GivenIHaveFinishedGettingTransfersForThatAccount()
        {
            WaitForTransferProcessingToComplete();
        }

        [Then(@"the payments processing completed message is created")]
        public void ThenThePaymentsProcessingCompletedMessageIsCreated()
        {
            WaitForPaymentsProcessingToComplete();
        }

        [Then(@"the account transfers should be save")]
        public void ThenTheAccountTransfersShouldBeSave()
        {
            var senderAccountId = (long)ScenarioContext.Current["AccountId"];
            var receiverAccount = (TestAccount)ScenarioContext.Current["RecieverAccount"];

            var transferRepository = _accountCreationSteps.Container.GetInstance<ITransferRepository>();

            var transfers = transferRepository.GetAccountTransfersByPeriodEnd(senderAccountId, _periodEnd.Id).Result;
            var transfer = transfers.Single();

            Assert.AreEqual(senderAccountId, transfer.SenderAccountId);
            Assert.AreEqual(receiverAccount.Id, transfer.ReceiverAccountId);
            Assert.AreEqual(receiverAccount.Name, transfer.ReceiverAccountName);
            Assert.AreEqual(_periodEnd.Id, transfer.PeriodEnd);
            Assert.AreEqual(DateTime.Now.ToShortDateString(), transfer.TransferDate.ToShortDateString());
            //Assert.AreEqual(TransferType.None, transfer.Type);
            Assert.AreEqual(TransferPaymentAmount, transfer.Amount);
            Assert.AreEqual(CourseName, transfer.CourseName);
        }


        [Then(@"the transfer senders transactions should be saved")]
        public void ThenTheTransferSendersTransactionsShouldBeSaved()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];

            var transactionRepository = _accountCreationSteps.Container.GetInstance<ITransactionRepository>();

            var fromDate = DateTime.Now.AddMinutes(-5);
            var toDate = DateTime.Now.AddMinutes(5);

            var transactions = transactionRepository.GetAccountTransactionsByDateRange(accountId, fromDate, toDate).Result;

            var transferTransactions = transactions.OfType<TransferTransactionLine>().ToArray();

            Assert.AreEqual(1, transferTransactions.Length);

            var transactionLine = transferTransactions.Single();
            var payment = _payments.Single();

            var transferRepository = _accountCreationSteps.Container.GetInstance<ITransferRepository>();

            var transfers = transferRepository.GetAccountTransfersByPeriodEnd(accountId, _periodEnd.Id).Result;

            var transfer = transfers.Single();

            var accountRepository = _accountCreationSteps.Container.GetInstance<IAccountRepository>();

            accountRepository.GetAccountNames(new List<long> { transfer.ReceiverAccountId });

            var expectedTransactionTotal = -payment.Amount;

            Assert.AreEqual(transfer.ReceiverAccountName, transactionLine.ReceiverAccountName);
            Assert.AreEqual(expectedTransactionTotal, transactionLine.Amount);
        }

        private void WaitForPaymentsProcessingToComplete()
        {
            _logger.Debug("Waiting for payments to complete");

            var factory = new TopicSubscriberFactory(_messageBusConnectionString, "MA_TransferDataProcessor_TEST", _logger);


            var subscriber = factory.GetSubscriber<AccountPaymentsProcessingCompletedMessage>();

            _logger.Debug("Subscribing to payments processing completed message queue");

            var task = subscriber.ReceiveAsAsync();

            _logger.Debug("Waiting for payment completed message...");

            var successful = task.Wait(new TimeSpan(0, 5, 0));

            if (!successful)
                Assert.Fail("Payment processing took too long");

            task.Result?.CompleteAsync().Wait();
        }

        private void WaitForTransferProcessingToComplete()
        {
            var factory = new TopicSubscriberFactory(_messageBusConnectionString, "MA_TransferTransactionProcessor_TEST", _logger);

            var subscriber = factory.GetSubscriber<AccountTransfersProcessingCompletedMessage>();

            var task = subscriber.ReceiveAsAsync();

            var successful = task.Wait(new TimeSpan(0, 10, 0));

            if (!successful)
                Assert.Fail("Transfer processing took too long");

            task.Result?.CompleteAsync().Wait();
        }

        private static void ClearSubscriptionMessageQueue(string connectionString, string topicName, string subscriptionName)
        {
            const int batchSize = 100;
            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName, ReceiveMode.ReceiveAndDelete);

            do
            {
                var messages = subscriptionClient.ReceiveBatch(batchSize, TimeSpan.FromTicks(100));
                if (!messages.Any())
                {
                    break;
                }
            }
            while (true);
        }
    }
}
