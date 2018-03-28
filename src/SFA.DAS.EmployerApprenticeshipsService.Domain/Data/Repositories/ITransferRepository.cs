using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferRepository
    {
        Task CreateAccountTransfers(IEnumerable<AccountTransfer> transfers);
        Task<AccountTransferDetails> GetTransferPaymentDetails(AccountTransfer transfer);
        Task<IEnumerable<AccountTransfer>> GetAccountTransfersByPeriodEnd(long senderAccountId, string periodEnd);
    }
}
