using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IPaymentService
    {
        Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, long employerAccountId, Guid correlationId);
        Task<IEnumerable<AccountTransfer>> GetAccountTransfers(string periodEnd, long receiverAccountId, Guid correlationId);       
    }
}
