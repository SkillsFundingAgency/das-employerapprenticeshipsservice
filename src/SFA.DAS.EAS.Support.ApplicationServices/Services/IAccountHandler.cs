﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services;

public interface IAccountHandler
{
    Task<AccountDetailOrganisationsResponse> FindOrganisations(string id);
    Task<AccountPayeSchemesResponse> FindPayeSchemes(string id);
    Task<AccountFinanceResponse> FindFinance(string id);
    Task<IEnumerable<AccountSearchModel>> FindAllAccounts(int pagesize, int pageNumber);
    Task<int> TotalAccountRecords(int pagesize);
    Task<AccountReponse> Find(string id);
    Task<AccountReponse> FindTeamMembers(string id);
    Task ChangeRole(string hashedAccountId, string email, int role, string supportUserEmail);
    Task ResendInvitation(string hashedAccountId, string email, string supportUserEmail);
}