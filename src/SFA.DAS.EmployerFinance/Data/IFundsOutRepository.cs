using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IFundsOutRepository
    {
        Task<IEnumerable<PaymentFundsOut>> GetFundsOut(long accountId);
    }
}