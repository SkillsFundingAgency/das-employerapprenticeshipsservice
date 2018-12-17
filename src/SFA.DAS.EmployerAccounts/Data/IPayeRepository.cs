﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IPayeRepository
    {
        Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId);
        Task AddPayeToAccount(Paye payeScheme);
        Task RemovePayeFromAccount(long accountId, string payeRef);
        Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string reference);
    }
}