﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewQueryHandler : IAsyncRequestHandler<GetAccountFinanceOverviewQuery, GetAccountFinanceOverviewResponse>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IDasForecastingService _dasForecastingService;
        private readonly IDasLevyService _levyService;
        private readonly IValidator<GetAccountFinanceOverviewQuery> _validator;
        private readonly ILog _logger;

        public GetAccountFinanceOverviewQueryHandler(
            ICurrentDateTime currentDateTime,
            IDasForecastingService dasForecastingService,
            IDasLevyService levyService,
            IValidator<GetAccountFinanceOverviewQuery> validator,
            ILog logger)
        {
            _currentDateTime = currentDateTime;
            _dasForecastingService = dasForecastingService;
            _levyService = levyService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<GetAccountFinanceOverviewResponse> Handle(GetAccountFinanceOverviewQuery query)
        {
            var validationResult = await _validator.ValidateAsync(query);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
           
            var currentBalance = await GetAccountBalance(query.AccountId.Value);
            var earliestFundsToExpire = await GetExpiringFunds(query.AccountId.Value);

            if (earliestFundsToExpire == null)
            {
                return new GetAccountFinanceOverviewResponse
                {
                    AccountId = query.AccountId.Value,
                    CurrentFunds = currentBalance
                };
            }

            return new GetAccountFinanceOverviewResponse
            {
                AccountId = query.AccountId.Value,
                CurrentFunds = currentBalance,
                ExpiringFundsExpiryDate = earliestFundsToExpire.PayrollDate,
                ExpiringFundsAmount = earliestFundsToExpire.Amount
            };
        }

        private async Task<ExpiringFunds> GetExpiringFunds(long accountId)
        {
            _logger.Info($"Getting expiring funds for account ID: {accountId}");
            
            var today = _currentDateTime.Now.Date;
            var nextYear = today.AddDays(1 - today.Day).AddMonths(13);
            var expiringFunds = await _dasForecastingService.GetExpiringAccountFunds(accountId);
            var earliestFundsToExpire = expiringFunds?.ExpiryAmounts?.Where(a => a.PayrollDate < nextYear).OrderBy(a => a.PayrollDate).FirstOrDefault();
            
            return earliestFundsToExpire;
        }

        private async Task<decimal> GetAccountBalance(long accountId)
        {
            try
            {
                _logger.Info($"Getting current funds balance for account ID: {accountId}");
                
                return await _levyService.GetAccountBalance(accountId);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Failed to get account's current balance for account ID: {accountId}");
                
                throw;
            }
        }
    }
}
