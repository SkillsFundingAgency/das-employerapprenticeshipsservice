using System.Collections.Generic;
using System.Text;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Formatters.TransactionDowloads
{
    public abstract class CsvTransactionFormatter
    {
        public string MimeType => "text/csv";

        public string FileExtension => "csv";

        public DownloadFormatType DownloadFormatType =>
            DownloadFormatType.CSV;

        public byte[] GetFileData(IEnumerable<TransactionDownloadLine> transactions)
        {
            var builder = new StringBuilder();

            builder.AppendLine(CreateHeaders());

            WriteRowsCsv(transactions, builder);
			
			var csvContent = builder.ToString();

            return Encoding.UTF8.GetBytes(csvContent);
        }

        protected abstract string CreateHeaders();

        protected abstract void WriteRowsCsv(IEnumerable<TransactionDownloadLine> transactions, StringBuilder builder);
    }
}