using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Formatters.TransactionDowloads
{
    public interface ITransactionFormatter
    {
        byte[] GetFileData(IEnumerable<TransactionDownloadLine> dataPayments);

        string MimeType { get; }

        string FileExtension { get; }

        DownloadFormatType DownloadFormatType { get; }
    }
}