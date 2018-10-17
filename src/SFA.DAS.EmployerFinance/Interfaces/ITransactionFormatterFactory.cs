using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface ITransactionFormatterFactory
    {
        ITransactionFormatter GetTransactionsFormatterByType(DownloadFormatType format);
    }
}