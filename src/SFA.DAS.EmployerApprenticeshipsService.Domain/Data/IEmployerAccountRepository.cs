﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEmployerAccountRepository
    {
        Task<Account> GetAccountById(long id);
        Task<Account> GetAccountByHashedId(string hashedId);
        Task<Accounts> GetAccounts(string toDate, int pageNumber, int pageSize);
    }
}
