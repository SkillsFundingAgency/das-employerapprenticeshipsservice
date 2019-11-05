﻿using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Formatters.TransactionDowloads
{
    public class LevyExcelTransactionFormatter : ExcelTransactionFormatter, ITransactionFormatter
    {
        public ApprenticeshipEmployerType ApprenticeshipEmployerType => ApprenticeshipEmployerType.Levy;

        public LevyExcelTransactionFormatter(IExcelService excelService) : base(excelService)
        {
        }

        protected override IEnumerable<string[]> GetTransactionRows(IEnumerable<TransactionDownloadLine> transactions)
        {
            return transactions.Select(transaction => new[]
            {
                transaction.DateCreated.ToString("G"),
                transaction.TransactionType,
                transaction.PayeScheme,
                transaction.PeriodEnd,
                transaction.LevyDeclaredFormatted,
                transaction.EnglishFractionFormatted,
                transaction.TenPercentTopUpFormatted,
                transaction.TrainingProvider,
                transaction.Uln,
                transaction.Apprentice,
                transaction.ApprenticeTrainingCourse,
                transaction.ApprenticeTrainingCourseLevel,
                transaction.PaidFromLevyFormatted,
                transaction.EmployerContributionFormatted,
                transaction.GovermentContributionFormatted,
                transaction.TotalFormatted
            });
        }

        protected override string[] GetHeaderRow()
        {
            return new[]{
                "Transaction date", "Transaction type", "PAYE scheme", "Payroll month", "Levy declared",
                "English %", "10% top up", "Training provider", "Unique learner number",
                "Apprentice", "Apprenticeship training course", "Course level", "Paid from levy", "Your contribution",
                "Government contribution", "Total"
            };
        }
    }
}