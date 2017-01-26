using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityById;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;

namespace SFA.DAS.EAS.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public AccountsOrchestrator(IMediator mediator, ILogger logger )
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
        {
            _logger.Info("Getting all account balances.");

            toDate = toDate ?? DateTime.MaxValue.ToString("yyyyMMddHHmmss");

            var accountsResult = await _mediator.SendAsync(new GetPagedEmployerAccountsQuery() {ToDate = toDate, PageSize = pageSize, PageNumber = pageNumber});
            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = accountsResult.Accounts.Select(account => account.Id).ToList()
            });

            var data = new List<AccountWithBalanceViewModel>();

            accountsResult.Accounts.ForEach(account =>
            {
                var accountBalance = transactionResult.Accounts.SingleOrDefault(c=>c.AccountId==account.Id);
                data.Add(new AccountWithBalanceViewModel { AccountId = account.Id, AccountName = account.Name, AccountHashId = account.HashedId, Balance = accountBalance?.Balance ?? 0 });
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>() {Data = new PagedApiResponseViewModel<AccountWithBalanceViewModel>() {Data = data, Page = pageNumber, TotalPages = (accountsResult.AccountsCount / pageSize) + 1} };
        }

        public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(string hashedAccountId)
        {
            var accountResult = await _mediator.SendAsync(new GetEmployerAccountByHashedIdQuery { HashedAccountId = hashedAccountId });
            if (accountResult.Account == null)
            {
                return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
            }

            var viewModel = ConvertAccountDetailToViewModel(accountResult);
            return new OrchestratorResponse<AccountDetailViewModel> { Data = viewModel };
        }

        public async Task<OrchestratorResponse<LegalEntityViewModel>> GetLegalEntity(long legalEntityId)
        {
            var legalEntityResult = await _mediator.SendAsync(new GetLegalEntityByIdQuery { Id = legalEntityId });
            if (legalEntityResult.LegalEntity == null)
            {
                return new OrchestratorResponse<LegalEntityViewModel> { Data = null };
            }

            var viewModel = ConvertLegalEntityToViewModel(legalEntityResult);
            return new OrchestratorResponse<LegalEntityViewModel> { Data = viewModel };
        }

        public async Task<OrchestratorResponse<PayeSchemeViewModel>> GetPayeScheme(string payeSchemeRef)
        {
            var payeSchemeResult = await _mediator.SendAsync(new GetPayeSchemeByRefQuery { Ref = payeSchemeRef });
            if (payeSchemeResult.PayeScheme == null)
            {
                return new OrchestratorResponse<PayeSchemeViewModel> { Data = null };
            }

            var viewModel = ConvertPayeSchemeToViewModel(payeSchemeResult);
            return new OrchestratorResponse<PayeSchemeViewModel> { Data = viewModel };
        }

        private PayeSchemeViewModel ConvertPayeSchemeToViewModel(GetPayeSchemeByRefResponse payeSchemeResult)
        {
            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                Name = payeSchemeResult.PayeScheme.Name,
                Ref = payeSchemeResult.PayeScheme.Ref
            };

            return payeSchemeViewModel;
        }

        private LegalEntityViewModel ConvertLegalEntityToViewModel(GetLegalEntityByIdResponse legalEntityResult)
        {
            var legalEntityViewModel = new LegalEntityViewModel
            {
                DateOfIncorporation = legalEntityResult.LegalEntity.DateOfIncorporation,
                Id = legalEntityResult.LegalEntity.Id,
                Name = legalEntityResult.LegalEntity.Name,
                Source = legalEntityResult.LegalEntity.Source,
                RegisteredAddress = legalEntityResult.LegalEntity.RegisteredAddress,
                Code = legalEntityResult.LegalEntity.Code,
                CompanyStatus = legalEntityResult.LegalEntity.CompanyStatus
            };

            return legalEntityViewModel;
        }

        private static AccountDetailViewModel ConvertAccountDetailToViewModel(GetEmployerAccountByHashedIdResponse accountResult)
        {
            var accountDetailViewModel = new AccountDetailViewModel
            {
                DasAccountId = accountResult.Account.HashedId,
                DateRegistered = accountResult.Account.CreatedDate,
                OwnerEmail = accountResult.Account.OwnerEmail,
                DasAccountName = accountResult.Account.Name,
                LegalEntities = new ResourceList(accountResult.Account.LegalEntities.Select(x => new ResourceViewModel { Id = x.ToString() })),
                PayeSchemes = new ResourceList(accountResult.Account.PayeSchemes.Select(x => new ResourceViewModel { Id = x }))
            };

            return accountDetailViewModel;
        }
    }
}