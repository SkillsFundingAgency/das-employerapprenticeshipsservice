using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Formatters.TransactionDowloads
{
    public interface ITransactionFormatter
    {
        byte[] GetFileData(IEnumerable<TransactionDownloadLine> dataPayments);

        string MimeType { get; }

        string FileExtension { get; }

        DownloadFormatType DownloadFormatType { get; }
    }
}