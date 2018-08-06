using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransactionRepository
    {
        Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transaction);
    }
}
