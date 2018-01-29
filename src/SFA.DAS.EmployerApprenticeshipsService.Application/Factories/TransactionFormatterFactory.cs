using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
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