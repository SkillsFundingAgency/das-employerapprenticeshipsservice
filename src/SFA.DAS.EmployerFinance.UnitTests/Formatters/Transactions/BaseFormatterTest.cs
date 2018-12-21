using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.UnitTests.Formatters.Transactions
{
    public abstract class BaseFormatterTest
    {
        public abstract ITransactionFormatter PaymentFormatter { get; }

        public abstract string ExpectedMimeType { get; }

        public abstract string ExpectedFileExtension { get; }

        public abstract DownloadFormatType ExpectedDownloadFormats { get; }

        protected const string ApprenticePrefix = "Apprentice";
        protected const string ApprenticeTrainingCoursePrefix = "ApprenticeTrainingCoursePrefix";
        protected const string ApprenticeTrainingCourseLevel = "666";
        protected const string EmpRefPrefix = "EmpRef";
        protected const string PeriodEndPrefix = "PeriodEnd";
        protected const string TrainingProviderPrefix = "TrainingProvider";
        protected const string CohortReferencePrefix = "CohortReference1";
        protected const string TransactionTypePrefix = "TransactionType1";
        protected const string UlnPrefix = "Uln1";


        protected List<TransactionDownloadLine> TransactionDownloadLines = new List<TransactionDownloadLine>
        {
            new TransactionDownloadLine
            {
                DateCreated = DateTime.Today.AddMonths(-1),
                Apprentice = $"{ApprenticePrefix}1",
                ApprenticeTrainingCourse = $"{ApprenticeTrainingCoursePrefix}1",
                ApprenticeTrainingCourseLevel = $"{ApprenticeTrainingCourseLevel}1",
                EmployerContribution = 1,
                PayeScheme = $"{EmpRefPrefix}1",
                PeriodEnd = $"{PeriodEndPrefix}1",
                PaidFromLevy = 10,
                TrainingProvider = $"{TrainingProviderPrefix}1",
                CohortReference = $"{CohortReferencePrefix}1",
                LevyDeclared = 1000,
                GovermentContribution = 10000,
                EnglishFraction = 0.1m,
                TenPercentTopUp = 100,
                Total = 1100,
                TransactionType = $"{TransactionTypePrefix}1",
                Uln = $"{UlnPrefix}1"
            },
            new TransactionDownloadLine
            {
                DateCreated = DateTime.Today.AddMonths(-2),
                Apprentice = $"{ApprenticePrefix}2",
                ApprenticeTrainingCourse = $"{ApprenticeTrainingCoursePrefix}2",
                ApprenticeTrainingCourseLevel = $"{ApprenticeTrainingCourseLevel}2",
                EmployerContribution = 2,
                PayeScheme = $"{EmpRefPrefix}2",
                PeriodEnd = $"{PeriodEndPrefix}2",
                PaidFromLevy = 20,
                TrainingProvider = $"{TrainingProviderPrefix}2",
                CohortReference = $"{CohortReferencePrefix}2",
                LevyDeclared = 2000,
                GovermentContribution = 20000,
                EnglishFraction = 0.2m,
                TenPercentTopUp = 200,
                Total = 2200,
                TransactionType = $"{TransactionTypePrefix}2",
                Uln = $"{UlnPrefix}2"
            },
            new TransactionDownloadLine
            {
                DateCreated = DateTime.Today.AddMonths(-3),
                Apprentice = $"{ApprenticePrefix}3",
                ApprenticeTrainingCourse = $"{ApprenticeTrainingCoursePrefix}3",
                ApprenticeTrainingCourseLevel = $"{ApprenticeTrainingCourseLevel}3",
                EmployerContribution = 3,
                PayeScheme = $"{EmpRefPrefix}3",
                PeriodEnd = $"{PeriodEndPrefix}3",
                PaidFromLevy = 30,
                TrainingProvider = $"{TrainingProviderPrefix}3",
                CohortReference = $"{CohortReferencePrefix}3",
                LevyDeclared = 3000,
                GovermentContribution = 30000,
                EnglishFraction = 0.3m,
                TenPercentTopUp = 300,
                Total = 3300,
                TransactionType = $"{TransactionTypePrefix}3",
                Uln = $"{UlnPrefix}3"
            },
        };

        [Test]
        public void CsvPaymentFormatterHasTheCorrectProperties()
        {
            Assert.AreEqual(ExpectedMimeType, PaymentFormatter.MimeType);
            Assert.AreEqual(ExpectedFileExtension, PaymentFormatter.FileExtension);
            Assert.AreEqual(ExpectedDownloadFormats, PaymentFormatter.DownloadFormatType);
        }
    }
}
