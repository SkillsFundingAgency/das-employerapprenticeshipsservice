using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IAccountRepository
    {
        Task<long> CreateAccount(string userId, string employerNumber, string employerName, string employerRef);
        Task<List<PayeView>> GetPayeSchemes(long accountId);
    }
}