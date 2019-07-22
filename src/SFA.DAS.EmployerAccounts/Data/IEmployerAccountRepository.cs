﻿using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerAccountRepository
    {
        Task<Account> GetAccountById(long id);
        Task<List<Account>> GetAllAccounts();
        Task<Account> GetAccountByHashedId(string hashedAccountId);
        Task<AccountStats> GetAccountStats(long accountId);
        Task RenameAccount(long id, string name);
        Task SetAccountAsLevy(long accountId);
    }
}
