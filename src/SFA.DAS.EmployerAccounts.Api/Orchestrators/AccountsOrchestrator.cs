using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AccountsOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<PayeSchemeViewModel> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            _logger.Info($"Getting paye scheme {payeSchemeRef} for account {hashedAccountId}");

            var payeSchemeResult = await _mediator.SendAsync(new GetPayeSchemeByRefQuery { HashedAccountId = hashedAccountId, Ref = payeSchemeRef });
            if (payeSchemeResult.PayeScheme == null)
            {
                return null;
            }

            var viewModel = ConvertPayeSchemeToViewModel(hashedAccountId, payeSchemeResult);
            return viewModel;
        }

        public async Task<OrchestratorResponse<PagedApiResponse<Account>>> GetAccounts(string toDate, int pageSize, int pageNumber)
        {
            _logger.Info("Getting all accounts.");

            toDate = toDate ?? DateTime.MaxValue.ToString("yyyyMMddHHmmss");

            var accountsResult = await _mediator.SendAsync(new GetPagedEmployerAccountsQuery { ToDate = toDate, PageSize = pageSize, PageNumber = pageNumber });            

            var data = new List<Account>();          

            accountsResult.Accounts.ForEach(account =>
            {
                var accountModel = new Account
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    AccountHashId = account.HashedId,
                    PublicAccountHashId = account.PublicHashedId,
                    IsLevyPayer = ((ApprenticeshipEmployerType) account.ApprenticeshipEmployerType) == ApprenticeshipEmployerType.Levy
                };

                data.Add(accountModel);
            });

            return new OrchestratorResponse<PagedApiResponse<Account>>
            {
                Data = new PagedApiResponse<Account>
                {
                    Data = data,
                    Page = pageNumber,
                    TotalPages = (accountsResult.AccountsCount / pageSize) + 1
                }
            };
        }

        private PayeSchemeViewModel ConvertPayeSchemeToViewModel(string hashedAccountId, GetPayeSchemeByRefResponse payeSchemeResult)
        {
            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                DasAccountId = hashedAccountId,
                Name = payeSchemeResult.PayeScheme.Name,
                Ref = payeSchemeResult.PayeScheme.Ref,
                AddedDate = payeSchemeResult.PayeScheme.AddedDate,
                RemovedDate = payeSchemeResult.PayeScheme.RemovedDate
            };

            return payeSchemeViewModel;
        }
    }
}