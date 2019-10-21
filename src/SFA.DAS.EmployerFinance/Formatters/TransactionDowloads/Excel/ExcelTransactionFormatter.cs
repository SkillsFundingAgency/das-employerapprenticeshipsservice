using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Formatters.TransactionDowloads
{
    public abstract class ExcelTransactionFormatter
    {
        private const string WorksheetName = "Transactions";
        private readonly IExcelService _excelService;

        public string MimeType => "text/xlsx";
        public string FileExtension => "xlsx";
        public DownloadFormatType DownloadFormatType => DownloadFormatType.Excel;

        public ExcelTransactionFormatter(IExcelService excelService)
        {
            _excelService = excelService;
        }

        protected abstract string[] GetHeaderRow();

        public byte[] GetFileData(IEnumerable<TransactionDownloadLine> transactions)
        {
            var excelRows = new List<string[]> { GetHeaderRow() };

            excelRows.AddRange(GetTransactionRows(transactions));

            var transactionData = new Dictionary<string, string[][]>
            {
                {WorksheetName, excelRows.ToArray()}
            };

            var fileData = _excelService.CreateExcelFile(transactionData);

            return fileData;
        }

        protected abstract IEnumerable<string[]> GetTransactionRows(IEnumerable<TransactionDownloadLine> transactions);
    }
}