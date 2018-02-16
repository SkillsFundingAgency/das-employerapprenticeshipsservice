using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public interface ITransactionFormatterFactory
    {
        ITransactionFormatter GetTransactionsFormatterByType(DownloadFormatType format);
    }
}