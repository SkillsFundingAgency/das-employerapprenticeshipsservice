using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Formatters.TransactionDowloads
{
    public class CsvTransactionFormatter : ITransactionFormatter
    {
        private static readonly string Headers = CreateHeaders();

        public string MimeType => "text/csv";

        public string FileExtension => "csv";

        public DownloadFormatType DownloadFormatType =>
            DownloadFormatType.Csv;

        public byte[] GetFileData(IEnumerable<TransactionDownloadLine> transactions)
        {
            var builder = new StringBuilder();

            builder.AppendLine(Headers);

            WriteRowsCsv(transactions, builder);
			
			var csvContent = builder.ToString();

            return Encoding.UTF8.GetBytes(csvContent);
        }

        private static string CreateHeaders()
        {
            var headerBuilder = new StringBuilder();
            headerBuilder.Append("Transaction date,");
            headerBuilder.Append("Transaction type,");
            headerBuilder.Append("PAYE scheme,");
            headerBuilder.Append("Payroll month,");
            headerBuilder.Append("Levy declared,");
            headerBuilder.Append("English %,");
            headerBuilder.Append("10% top up,");
            headerBuilder.Append("Training provider,");
            headerBuilder.Append("Cohort reference,");
            headerBuilder.Append("Unique learner number,");
            headerBuilder.Append("Apprentice,");
            headerBuilder.Append("Apprenticeship training course,");
            headerBuilder.Append("Paid from levy,");
            headerBuilder.Append("Your contribution,");
            headerBuilder.Append("Government contribution,");
            headerBuilder.Append("Total,");

            return headerBuilder.ToString();
        }

        private static void WriteRowsCsv(IEnumerable<TransactionDownloadLine> transactions, StringBuilder builder)
        {
            foreach (var transaction in transactions)
            {
                builder.Append($"{transaction.DateCreated:G},");
                builder.Append($"{transaction.TransactionType},");
                builder.Append($"{transaction.EmpRef},");
                builder.Append($"{transaction.PeriodEnd},");
                builder.Append($"{transaction.LevyDeclared.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.Append($"{transaction.EnglishFraction.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.Append($"{transaction.TenPercentTopUp.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.Append($"{transaction.TrainingProvider},");
                builder.Append($"{transaction.CohortReference},");
                builder.Append($"{transaction.Uln},");
                builder.Append($"{transaction.Apprentice},");
                builder.Append($"{transaction.ApprenticeTrainingCourse}{transaction.ApprenticeTrainingCourseLevel},");
                builder.Append($"{transaction.PaidFromLevy.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.Append($"{transaction.EmployerContribution.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.Append($"{transaction.GovermentContribution.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.Append($"{transaction.Total.ToString(NumberFormatInfo.InvariantInfo)},");
                builder.AppendLine();
            }

            // Get rid of last new line
            builder.Remove(builder.Length - 1, 1);
        }
    }
}