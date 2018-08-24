using SFA.DAS.EmployerFinance.Models.AccountTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IMembershipRepository
    {
        Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId);
    }
}
