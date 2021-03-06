﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IAccountRepository
    {
        Task<string> GetAccountName(long accountId);
        Task<Dictionary<long, string>> GetAccountNames(IEnumerable<long> accountIds);
        Task CreateAccount(long accountId, string name);
        Task RenameAccount(long accountId, string name);
    }
}
