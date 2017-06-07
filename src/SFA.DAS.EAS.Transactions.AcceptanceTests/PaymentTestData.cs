using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using System;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.Provider.Events.Api.Types;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests
{
    public class PaymentTestData
    {
        private readonly IContainer _container;
        
        public PaymentTestData(IContainer container)
        {
            _container = container;
        }

        public long AccountId
        {
            get { return (long) ScenarioContext.Current["AccountId"]; }
            set
            {
                ScenarioContext.Current["AccountId"] = value;

                if (StandardPayment != null)
                {
                    StandardPayment.EmployerAccountId = value.ToString();
                }

                if (FrameworkPayment != null)
                {
                    FrameworkPayment.EmployerAccountId = value.ToString();
                }

                if (StandardSFACoInvestmentPayment != null)
                {
                    StandardSFACoInvestmentPayment.EmployerAccountId = value.ToString();
                }

                if (StandardEmployerCoInvestmentPayment != null)
                {
                    StandardEmployerCoInvestmentPayment.EmployerAccountId = value.ToString();
                }
            }
        }

        public Apprenticeship Apprenticeship
        {
            get { return GetProperty<Apprenticeship>("Apprenticeship"); }
            set
            {
                value.EmployerAccountId = AccountId;
                ScenarioContext.Current["Apprenticeship"] = value;
            }
        }

        public Standard Standard
        {
            get { return GetProperty<Standard>("Standard"); }
            set { ScenarioContext.Current["Standard"] = value; }
        }

        public StandardsView StandardsView
        {
            get { return StandardObjectMother.CreateView(Standard); }
        }

        public Framework Framework
        {
            get { return GetProperty<Framework>("Framework"); }
            set { ScenarioContext.Current["Framework"] = value; }
        }

        public FrameworksView FrameworksView
        {
            get { return FrameworkObjectMother.CreateView(Framework); }
        }

        public Domain.Models.ApprenticeshipProvider.Provider Provider
        {
            get { return GetProperty<Domain.Models.ApprenticeshipProvider.Provider>("Provider"); }
            set { ScenarioContext.Current["Provider"] = value; }
        }

        public ProvidersView ProvidersView
        {
            get
            {
                return new ProvidersView
                {
                    CreatedDate = DateTime.Now,
                    Provider = Provider
                };
            }
        }

        public DateTime PaymentDeliveryDate
        {
            get { return GetProperty<DateTime>("PaymentDeliveryDate"); }
            set { ScenarioContext.Current["PaymentDeliveryDate"] = value; }
        }

        public DateTime PaymentCollectionDate
        {
            get { return GetProperty<DateTime>("PaymentCollectionDate"); }
            set { ScenarioContext.Current["PaymentCollectionDate"] = value; }
        }

        public Payment StandardPayment
        {
            get { return GetProperty<Payment>("StandardPayment"); }
            set { ScenarioContext.Current["StandardPayment"] = value; }
        }

        public Payment StandardSFACoInvestmentPayment
        {
            get { return GetProperty<Payment>("StandardSFAPayment"); }
            set { ScenarioContext.Current["StandardSFAPayment"] = value; }
        }

        public Payment StandardEmployerCoInvestmentPayment
        {
            get { return GetProperty<Payment>("StandardEmployerPayment"); }
            set { ScenarioContext.Current["StandardEmployerPayment"] = value; }
        }

        public Payment FrameworkPayment
        {
            get { return GetProperty<Payment>("FrameworkPayment"); }
            set { ScenarioContext.Current["FrameworkPayment"] = value; }
        }

        public Domain.Models.Payments.PeriodEnd PeriodEnd
        {
            get { return GetProperty<Domain.Models.Payments.PeriodEnd>("periodEnd"); }
            set { ScenarioContext.Current["periodEnd"] = value; }
        }

        public void PopulateTestData()
        {
            AccountId = 1L;

            Apprenticeship = ApprenticeshipObjectMother.Create("John", "Doe");

            Standard = StandardObjectMother.Create("Testing");

            Framework = FrameworkObjectMother.Create("Testing", "General");

            Provider = new Domain.Models.ApprenticeshipProvider.Provider
            {
                ProviderName = "Test Corp"
            };

            //Simulating a payment for last month fee of a course that has been submitted this month 
            PaymentDeliveryDate = DateTime.Now.AddMonths(-1);
            PaymentCollectionDate = DateTime.Now;

            PeriodEnd = GetCurrentMonthPeriodEnd();

            StandardPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 100,
                StandardCode = Standard.Code,
                ApprenticeshipId = Apprenticeship.Id,
                DeliveryPeriod = new CalendarPeriod { Month = PaymentDeliveryDate.Month, Year = PaymentDeliveryDate.Year },
                Amount = 200,
                CollectionPeriod = new NamedCalendarPeriod { Id = PeriodEnd.Id, Month = PaymentCollectionDate.Month, Year = PaymentCollectionDate.Year },
                TransactionType = TransactionType.Learning,
                EvidenceSubmittedOn = DateTime.Now.AddDays(-5),
                Uln = 123,
                EmployerAccountVersion = "1.0",
                EmployerAccountId = AccountId.ToString(),
                FundingSource = FundingSource.Levy,
                ApprenticeshipVersion = "1.0"
            };

            StandardSFACoInvestmentPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 100,
                StandardCode = Standard.Code,
                ApprenticeshipId = Apprenticeship.Id,
                DeliveryPeriod = new CalendarPeriod { Month = DateTime.Now.AddMonths(-1).Month, Year = DateTime.Now.AddMonths(-1).Year },
                Amount = 90,
                CollectionPeriod = new NamedCalendarPeriod { Id = PeriodEnd.Id, Month = DateTime.Now.Month, Year = DateTime.Now.Year },
                TransactionType = TransactionType.Learning,
                EvidenceSubmittedOn = DateTime.Now.AddDays(-5),
                Uln = 123,
                EmployerAccountVersion = "1.0",
                EmployerAccountId = AccountId.ToString(),
                FundingSource = FundingSource.CoInvestedSfa,
                ApprenticeshipVersion = "1.0"

            };

            StandardEmployerCoInvestmentPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 100,
                StandardCode = Standard.Code,
                ApprenticeshipId = Apprenticeship.Id,
                DeliveryPeriod = new CalendarPeriod { Month = DateTime.Now.AddMonths(-1).Month, Year = DateTime.Now.AddMonths(-1).Year },
                Amount = 10,
                CollectionPeriod = new NamedCalendarPeriod { Id = PeriodEnd.Id, Month = DateTime.Now.Month, Year = DateTime.Now.Year },
                TransactionType = TransactionType.Learning,
                EvidenceSubmittedOn = DateTime.Now.AddDays(-5),
                Uln = 123,
                EmployerAccountVersion = "1.0",
                EmployerAccountId = AccountId.ToString(),
                FundingSource = FundingSource.CoInvestedEmployer,
                ApprenticeshipVersion = "1.0"
            };

            FrameworkPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 100,
                FrameworkCode = Framework.FrameworkCode,
                ProgrammeType = Framework.ProgrammeType,
                PathwayCode = Framework.PathwayCode,
                ApprenticeshipId = Apprenticeship.Id,
                DeliveryPeriod = new CalendarPeriod { Month = PaymentDeliveryDate.Month, Year = PaymentDeliveryDate.Year },
                Amount = 200,
                CollectionPeriod = new NamedCalendarPeriod { Id = PeriodEnd.Id, Month = PaymentCollectionDate.Month, Year = PaymentCollectionDate.Year },
                TransactionType = TransactionType.Learning,
                EvidenceSubmittedOn = DateTime.Now.AddDays(-5),
                Uln = 123,
                EmployerAccountVersion = "1.0",
                EmployerAccountId = AccountId.ToString(),
                FundingSource = FundingSource.Levy,
                ApprenticeshipVersion = "1.0"
            };
        }
        
        private Domain.Models.Payments.PeriodEnd GetCurrentMonthPeriodEnd()
        {
            var repository = _container.GetInstance<IDasLevyRepository>();

            var latestPeriodEnd = repository.GetLatestPeriodEnd().Result;

            //If a period end for this month already exists then use it
            if (latestPeriodEnd != null &&
                latestPeriodEnd.CalendarPeriodMonth == DateTime.Now.Month &&
                latestPeriodEnd.CalendarPeriodYear == DateTime.Now.Year)
            {
                return latestPeriodEnd;
            }

            //Else creates a new period end for this month 
            var periodEnd = new Domain.Models.Payments.PeriodEnd
            {
                Id = DateTime.Now.ToString("ddmmyyHHMMss"),
                CalendarPeriodMonth = DateTime.Now.Month,
                CalendarPeriodYear = DateTime.Now.Year,
                CompletionDateTime = DateTime.Now,
                AccountDataValidAt = DateTime.Now,
                CommitmentDataValidAt = DateTime.Now,
                PaymentsForPeriod = string.Empty
            };

            repository.CreateNewPeriodEnd(periodEnd).Wait();

            return periodEnd;
        }

        private static T GetProperty<T>(string name) 
        {
            return ScenarioContext.Current.ContainsKey(name) ? (T)ScenarioContext.Current[name] : default(T);
        }
    }
}
