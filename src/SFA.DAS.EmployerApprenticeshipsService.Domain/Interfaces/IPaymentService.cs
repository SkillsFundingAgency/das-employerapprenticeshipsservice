using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, long employerAccountId);
        Task<IEnumerable<AccountTransfer>> GetAccountTransfers(string periodEnd, long receiverAccountId);
    }
}
