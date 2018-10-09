using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;
using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.Formatters
{
    public class TransactionFormatterFactory : ITransactionFormatterFactory
    {
        private readonly IEnumerable<ITransactionFormatter> _formatters;

        public TransactionFormatterFactory(IEnumerable<ITransactionFormatter> formatters)
        {
            _formatters = formatters;
        }

        public ITransactionFormatter GetTransactionsFormatterByType(DownloadFormatType format)
        {
            return _formatters.FirstOrDefault(f => f.DownloadFormatType == format);
        }
    }
}