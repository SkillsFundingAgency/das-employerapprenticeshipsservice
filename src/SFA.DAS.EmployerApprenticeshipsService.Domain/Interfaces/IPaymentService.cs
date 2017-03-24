using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, long employerAccountId);
    }
}
