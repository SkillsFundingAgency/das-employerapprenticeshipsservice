﻿using System;
using System.Threading.Tasks;

namespace SFA.DAS.Account.Api.Client
{
    public interface IAccountApiClient
    {
        Task<Dtos.PagedApiResponseViewModel<Dtos.AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null);
    }
}