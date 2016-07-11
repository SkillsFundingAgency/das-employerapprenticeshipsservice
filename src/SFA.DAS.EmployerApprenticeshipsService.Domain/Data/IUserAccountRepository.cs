using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IUserAccountRepository : IRepository
    {
        Task<List<Account>> GetAccountsByUserId(string userId);
    }
}
