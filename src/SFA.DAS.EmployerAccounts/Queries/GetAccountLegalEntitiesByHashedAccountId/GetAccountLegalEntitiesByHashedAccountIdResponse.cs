﻿using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId
{
    public class GetAccountLegalEntitiesByHashedAccountIdResponse
    {
        public List<AccountSpecificLegalEntity> LegalEntities { get; set; }
    }
}