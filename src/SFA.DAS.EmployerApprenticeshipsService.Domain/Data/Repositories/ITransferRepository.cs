using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferRepository
    {
        Task<decimal> GetTransferAllowance(long accountId);
        Task CreateAccountTransfers(IEnumerable<AccountTransfer> transfers);
        Task<AccountTransferPaymentDetails> GetTransferPaymentDetails(AccountTransfer transfer);
    }
}
