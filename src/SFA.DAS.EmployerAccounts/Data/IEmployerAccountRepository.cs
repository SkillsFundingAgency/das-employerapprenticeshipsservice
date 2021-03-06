﻿using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerAccountRepository
    {
        Task<Account> GetAccountById(long id);
        Task<List<Account>> GetAllAccounts();
        Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize);
        Task<Account> GetAccountByHashedId(string hashedAccountId);
        Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId);
        Task<AccountStats> GetAccountStats(long accountId);
        Task RenameAccount(long id, string name);
        Task SetAccountLevyStatus(long accountId, ApprenticeshipEmployerType apprenticeshipEmployerType);
    }
}
