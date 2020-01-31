using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Formatters.TransactionDowloads
{
    public class NonLevyExcelTransactionFormatter : ExcelTransactionFormatter, ITransactionFormatter
    {
        public ApprenticeshipEmployerType ApprenticeshipEmployerType => ApprenticeshipEmployerType.NonLevy;

        public NonLevyExcelTransactionFormatter(IExcelService excelService) : base(excelService)
        {
        }

        protected override IEnumerable<string[]> GetTransactionRows(IEnumerable<TransactionDownloadLine> transactions)
        {
            return transactions.Select(transaction => new[]
            {
                transaction.DateCreated.ToString("G"),
                transaction.TransactionType,
                transaction.Description,
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
                "Transaction date", "Transaction type", "Description", "Training provider", "Unique learner number",
                "Apprentice", "Apprenticeship training course", "Course level", "Paid from transfer", "Your contribution",
                "Government contribution", "Total"
            };
        }
    }
}