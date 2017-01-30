﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public interface IAccountApiClient
    {
        Task<Dtos.PagedApiResponseViewModel<Dtos.AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null);

        /// <summary>
        /// Gets account information for a specific date range
        /// </summary>
        /// <param name="fromDate">The date accounts created from</param>
        /// <param name="toDate">The date accounts created to</param>
        /// <param name="pageNumber">Page number to get</param>
        /// <param name="pageSize">Number of records per page</param>
        Task<Dtos.PagedApiResponseViewModel<Dtos.AccountInformationViewModel>> GetPageOfAccountInformation(DateTime fromDate, DateTime toDate,int pageNumber = 1, int pageSize = 1000);

        /// <summary>
        /// Gets account information for a specific account.
        /// </summary>
        /// <param name="accountId">The account to get information for</param>
        Task<List<Dtos.AccountInformationViewModel>> GetAccountInformationById(string accountId);
    }
}