using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransferRepository
    {
        Task CreateAccountTransfersV1(IEnumerable<AccountTransfer> transfers);
        Task CreateAccountTransfers(IEnumerable<AccountTransfer> transfers);
        Task<IEnumerable<AccountTransfer>> GetReceiverAccountTransfersByPeriodEnd(long receiverAccountId, string periodEnd);
        Task<AccountTransferDetails> GetTransferPaymentDetails(AccountTransfer transfer);
    }
}
