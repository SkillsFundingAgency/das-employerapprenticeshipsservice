using System.Collections.Generic;
using System.Text;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Formatters.TransactionDowloads
{
    public class NonLevyCsvTransactionFormatter : CsvTransactionFormatter, ITransactionFormatter
    {
        public ApprenticeshipEmployerType ApprenticeshipEmployerType => ApprenticeshipEmployerType.NonLevy;

        protected override string CreateHeaders()
        {
            var headerBuilder = new StringBuilder();
            headerBuilder.Append("Transaction date,");
            headerBuilder.Append("Transaction type,");
            headerBuilder.Append("Description,");
            headerBuilder.Append("Training provider,");
            headerBuilder.Append("Unique learner number,");
            headerBuilder.Append("Apprentice,");
            headerBuilder.Append("Apprenticeship training course,");
            headerBuilder.Append("Course level,");
            headerBuilder.Append("Paid from transfer,");
            headerBuilder.Append("Your contribution,");
            headerBuilder.Append("Government contribution,");
            headerBuilder.Append("Total,");

            return headerBuilder.ToString();
        }

        protected override void WriteRowsCsv(IEnumerable<TransactionDownloadLine> transactions, StringBuilder builder)
        {
            foreach (var transaction in transactions)
            {
                builder.Append($"{transaction.DateCreated:dd/MM/yyyy},");
                builder.Append($"{transaction.TransactionType},");
                builder.Append($"{transaction.Description},");
                builder.Append($"{transaction.TrainingProviderFormatted},");
                builder.Append($"{transaction.Uln},");
                builder.Append($"{transaction.Apprentice},");
                builder.Append($"{transaction.ApprenticeTrainingCourse},");
                builder.Append($"{transaction.ApprenticeTrainingCourseLevel},");
                builder.Append($"{transaction.PaidFromLevyFormatted},");
                builder.Append($"{transaction.EmployerContributionFormatted},");
                builder.Append($"{transaction.GovermentContributionFormatted},");
                builder.Append($"{transaction.TotalFormatted},");
                builder.AppendLine();
            }

            // Get rid of last new line
            builder.Remove(builder.Length - 1, 1);
        }
    }
}
