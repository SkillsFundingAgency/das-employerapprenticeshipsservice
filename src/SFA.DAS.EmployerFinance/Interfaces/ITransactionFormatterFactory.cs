using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface ITransactionFormatterFactory
    {
        ITransactionFormatter GetTransactionsFormatterByType(DownloadFormatType format, ApprenticeshipEmployerType apprenticeshipEmployerType);
    }
}