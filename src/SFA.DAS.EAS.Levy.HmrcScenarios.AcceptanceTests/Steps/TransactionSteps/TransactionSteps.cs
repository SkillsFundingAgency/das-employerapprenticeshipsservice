﻿using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.TestCommon.DbCleanup;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.Provider.Events.Api.Types;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Levy.HmrcScenarios.AcceptanceTests2.Steps.TransactionSteps
{
    [Binding, Explicit]
    public class TransactionSteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IAuthenticationService> _owinWrapper;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private static Mock<IEmployerCommitmentApi> _commitmentsApi;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IAuthenticationService>();
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

        [When(@"I have the following submissions")]
        public void WhenIHaveTheFollowingSubmissions(Table table)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var dasLevyRepository = _container.GetInstance<IDasLevyRepository>();
            //For each row in the table insert into the levy_Declarations table
            var lineCount = 1;

            var emprefDictionary = new Dictionary<string,decimal>();

            foreach (var tableRow in table.Rows)
            {
                var dasDeclaration = new DasDeclaration
                {
                    LevyDueYtd = Convert.ToDecimal(tableRow["LevyDueYtd"]),
                    PayrollYear = tableRow["Payroll_Year"],
                    PayrollMonth = Convert.ToInt16(tableRow["Payroll_Month"]),
                    LevyAllowanceForFullYear = 15000,
                    SubmissionDate = DateTime.Parse(tableRow["SubmissionDate"]),
                    Id = lineCount.ToString()
                };
                lineCount++;
                if (!emprefDictionary.ContainsKey(tableRow["Paye_scheme"]))
                {
                    emprefDictionary.Add(tableRow["Paye_scheme"],Convert.ToDecimal(tableRow["English_Fraction"]));
                }
                if (tableRow.ContainsKey("EndOfYearAdjustment"))
                {
                    dasDeclaration.EndOfYearAdjustment = Convert.ToInt16(tableRow["EndOfYearAdjustment"]) == 1;
                }
                if (tableRow.ContainsKey("EndOfYearAdjustmentAmount"))
                {
                    dasDeclaration.EndOfYearAdjustmentAmount = Convert.ToDecimal(tableRow["EndOfYearAdjustmentAmount"]);
                }

                dasLevyRepository.CreateEmployerDeclarations(new List<DasDeclaration>{ dasDeclaration }, tableRow["Paye_scheme"], accountId).Wait();
            }

            var englishFractionRepository = _container.GetInstance<IEnglishFractionRepository>();

            foreach (var empRef in emprefDictionary)
            {
                englishFractionRepository.CreateEmployerFraction(new DasEnglishFraction
                {
                    Amount = empRef.Value,
                    DateCalculated = new DateTime(2016, 01, 01),
                    EmpRef = empRef.Key
                },empRef.Key).Wait();

                dasLevyRepository.ProcessDeclarations(accountId, empRef.Key).Wait();
            }

            lineCount = 1;

            var updateTransactionLine = _container.GetInstance<IUpdateTransactionLine>();

            foreach (var tableRow in table.Rows)
            {
                var subId = lineCount;
                
                if (tableRow.ContainsKey("CreatedDate") && !string.IsNullOrEmpty(tableRow["CreatedDate"]))
                {
                    updateTransactionLine.Execute(subId, DateTime.Parse(tableRow["CreatedDate"])).Wait();
                }
                lineCount++;

            }

        }

        [When(@"I have the following payments")]
        public void WhenIHaveTheFollowingPayments(Table table)
        {
            var accountId = (long) ScenarioContext.Current["AccountId"];
            var dasLevyRepository = _container.GetInstance<IDasLevyRepository>();

            var paymentsList = new List<PaymentDetails>();

            foreach (var tableRow in table.Rows)
            {
                var payment = new PaymentDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = Convert.ToDecimal(tableRow["Payment_Amount"]),
                    TransactionType = TransactionType.Learning,
                    ProgrammeType = tableRow["Payment_Type"].ToLower().Equals("levy") ? 1 : 2,
                    DeliveryPeriodMonth = 1,
                    DeliveryPeriodYear = 2016,
                    CollectionPeriodId = "1617-R12",
                    CollectionPeriodMonth = 1,
                    CollectionPeriodYear = 2016,
                    FundingSource =
                        tableRow["Payment_Type"].ToLower().Equals("levy")
                            ? FundingSource.Levy
                            : FundingSource.CoInvestedEmployer,
                    EvidenceSubmittedOn = DateTime.Now,
                    EmployerAccountVersion = "123",
                    ApprenticeshipId = 1,
                    ApprenticeshipVersion = "123",
                    EmployerAccountId = accountId,
                    Ukprn = 1,
                    Uln = 1,
                    FrameworkCode = 1,
                    PathwayCode = 1,
                    StandardCode = 1,
                    ProviderName = "Provider 1",
                    CourseName = "Course 1",
                    PeriodEnd = "1617-R12"
                };

                paymentsList.Add(payment);
            }

            dasLevyRepository.CreatePaymentData(paymentsList).Wait();

            ScenarioContext.Current["payments"] = paymentsList;

            dasLevyRepository.CreateNewPeriodEnd(new Domain.Models.Payments.PeriodEnd
            {
                CalendarPeriodMonth = 1,
                CalendarPeriodYear = 2016,
                CompletionDateTime = DateTime.Now,
                Id = "1617-R12",
                AccountDataValidAt = DateTime.Now,
                CommitmentDataValidAt = DateTime.Now,
                PaymentsForPeriod = string.Empty
            }).Wait();

            dasLevyRepository.ProcessPaymentData(accountId).Wait();
        }


        [Then(@"the balance should be (.*) on the screen")]
        public void ThenTheBalanceShouldBeOnTheScreen(decimal balance)
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();
            
            var actual = employerAccountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId,DateTime.Now.Year, DateTime.Now.Month, userId).Result;

            Assert.AreEqual(balance,actual.Data.Model.CurrentBalance);
        }

        [When(@"I register on month (.*)")]
        public void WhenIRegisterOnDASInMonth(string p0)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
