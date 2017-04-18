using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Payments.Events.Api.Types;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.TransactionSteps
{
    [Binding, Explicit]
    public class TransactionSteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private string _hashedAccountId;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi);

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

                dasLevyRepository.CreateEmployerDeclaration(dasDeclaration, tableRow["Paye_scheme"], accountId).Wait();
                
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
            }

            dasLevyRepository.ProcessDeclarations().Wait();


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
                    DeliveryPeriod = new CalendarPeriod {Month = 1, Year = 2016},
                    CollectionPeriod = new NamedCalendarPeriod {Id = "1617-R12", Month = 1, Year = 2016},
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

                dasLevyRepository.CreatePaymentData(payment).Wait();
            }

            ScenarioContext.Current["payments"] = paymentsList;

            dasLevyRepository.CreateNewPeriodEnd(new PeriodEnd
            {
                CalendarPeriod = new CalendarPeriod {Month = 1, Year = 2016},
                CompletionDateTime = DateTime.Now,
                Id = "1617-R12",
                ReferenceData = new ReferenceDataDetails { AccountDataValidAt = DateTime.Now,CommitmentDataValidAt = DateTime.Now},
                Links = new PeriodEndLinks { PaymentsForPeriod = ""}
            }).Wait();


            dasLevyRepository.ProcessPaymentData().Wait();
        }


        [Then(@"the balance should be (.*) on the screen")]
        public void ThenTheBalanceShouldBeOnTheScreen(decimal balance)
        {
            var employerAccountTransactionsOrchestrator = _container.GetInstance<EmployerAccountTransactionsOrchestrator>();
            var hashedAccountId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserId"].ToString();

            var actual = employerAccountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, userId).Result;

            Assert.AreEqual(balance,actual.Data.Model.CurrentBalance);
        }

        [When(@"I register on month (.*)")]
        public void WhenIRegisterOnDASInMonth(string p0)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
