using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Formatters.TransactionDowloads
{
    public class ExcelTransactionFormatter : ITransactionFormatter
    {
        private const string WorksheetName = "Transactions";
        private static readonly string [] HeaderRow = {
            "Transaction date", "Transaction type", "PAYE scheme", "Payroll month", "Levy declared",
            "English %", "10% top up", "Training provider", "Unique learner number",
            "Apprentice", "Apprenticeship training course", "Paid from levy", "Your contribution",
            "Government contribution", "Total"
        };
        private readonly IExcelService _excelService;

        public string MimeType => "text/xlsx";
        public string FileExtension => "xlsx";
        public DownloadFormatType DownloadFormatType => DownloadFormatType.Excel;

        public ExcelTransactionFormatter(IExcelService excelService)
        {
            _excelService = excelService;
        }

        public byte[] GetFileData(IEnumerable<TransactionDownloadLine> transactions)
        {
            var excelRows = new List<string[]> { HeaderRow };

            excelRows.AddRange(transactions.Select(transaction => new[]
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
                transaction.PaidFromLevyFormatted,
                transaction.EmployerContributionFormatted,
                transaction.GovermentContributionFormatted,
                transaction.TotalFormatted
            }));

            var transactionData = new Dictionary<string, string[][]>
            {
                {WorksheetName, excelRows.ToArray()}
            };

            var fileData = _excelService.CreateExcelFile(transactionData);

            return fileData;
        }
    }
}