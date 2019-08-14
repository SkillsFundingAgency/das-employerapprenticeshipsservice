﻿using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclarationsByAccountAndPeriod;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.GetTeamMembers;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;

        public AccountsOrchestrator(IMediator mediator, ILog logger, IMapper mapper, IHashingService hashingService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hashingService = hashingService;
        }

        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
        {
            _logger.Info("Getting all account balances.");

            toDate = toDate ?? DateTime.MaxValue.ToString("yyyyMMddHHmmss");

            var accountsResult = await _mediator.SendAsync(new GetPagedEmployerAccountsQuery { ToDate = toDate, PageSize = pageSize, PageNumber = pageNumber });
            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = accountsResult.Accounts.Select(account => account.Id).ToList()
            });

            var data = new List<AccountWithBalanceViewModel>();

            var accountBalanceHash = BuildAccountBalanceHash(transactionResult.Accounts);

            accountsResult.Accounts.ForEach(account =>
            {
                var accountBalanceModel = new AccountWithBalanceViewModel
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    AccountHashId = account.HashedId,
                    PublicAccountHashId = account.PublicHashedId,
                    IsLevyPayer = true
                };

                if (accountBalanceHash.TryGetValue(account.Id, out var accountBalance))
                {
                    accountBalanceModel.Balance = accountBalance.Balance;
                    accountBalanceModel.RemainingTransferAllowance = accountBalance.RemainingTransferAllowance;
                    accountBalanceModel.StartingTransferAllowance = accountBalance.StartingTransferAllowance;
                    accountBalanceModel.IsLevyPayer = accountBalance.IsLevyPayer == 1;
                }

                data.Add(accountBalanceModel);
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>> { Data = new PagedApiResponseViewModel<AccountWithBalanceViewModel> { Data = data, Page = pageNumber, TotalPages = (accountsResult.AccountsCount / pageSize) + 1 } };
        }

        private Dictionary<long, AccountBalance> BuildAccountBalanceHash(List<AccountBalance> accountBalances)
        {
            var result = new Dictionary<long, AccountBalance>(accountBalances.Count);

            foreach (var balance in accountBalances)
            {
                result.Add(balance.AccountId, balance);
            }

            return result;
        }

        public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);

            if (string.IsNullOrWhiteSpace(hashedAccountId))
            {
                return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
            }

            var response = await GetAccount(hashedAccountId);
            return response;
        }

        public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(string hashedAccountId)
        {
            _logger.Info($"Getting account {hashedAccountId}");

            var accountResult = await _mediator.SendAsync(new GetEmployerAccountByHashedIdQuery { HashedAccountId = hashedAccountId });
            if (accountResult.Account == null)
            {
                return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
            }

            var viewModel = ConvertAccountDetailToViewModel(accountResult);

            var accountBalanceTask = GetBalanceForAccount(accountResult.Account.AccountId);
            var transferBalanceTask = GetTransferAllowanceForAccount(accountResult.Account.AccountId);

            await Task.WhenAll(accountBalanceTask, transferBalanceTask).ConfigureAwait(false);

            viewModel.Balance = accountBalanceTask.Result;
            viewModel.RemainingTransferAllowance = transferBalanceTask.Result.RemainingTransferAllowance ?? 0;
            viewModel.StartingTransferAllowance = transferBalanceTask.Result.StartingTransferAllowance ?? 0;

            return new OrchestratorResponse<AccountDetailViewModel> { Data = viewModel };
        }

        public async Task<OrchestratorResponse<ICollection<TeamMemberViewModel>>> GetAccountTeamMembers(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);

            var response = await GetAccountTeamMembers(hashedAccountId);

            return response;

        }

        public async Task<OrchestratorResponse<ICollection<TeamMemberViewModel>>> GetAccountTeamMembers(string hashedAccountId)
        {
            _logger.Info($"Requesting team members for account {hashedAccountId}");

            var teamMembers = await _mediator.SendAsync(new GetTeamMembersRequest { HashedAccountId = hashedAccountId });

            var memberViewModels = teamMembers.TeamMembers.Select(x => _mapper.Map<TeamMemberViewModel>(x)).ToList();

            return new OrchestratorResponse<ICollection<TeamMemberViewModel>>
            {
                Data = memberViewModels,
                Status = HttpStatusCode.OK
            };
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}");

            var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationRequest { HashedAccountId = hashedAccountId });
            if (levyDeclarations.Declarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Declarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth}");

            var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = hashedAccountId, PayrollYear = payrollYear, PayrollMonth = payrollMonth });
            if (levyDeclarations.Declarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Declarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }

        private static AccountDetailViewModel ConvertAccountDetailToViewModel(GetEmployerAccountByHashedIdResponse accountResult)
        {
            var accountDetailViewModel = new AccountDetailViewModel
            {
                AccountId = accountResult.Account.AccountId,
                HashedAccountId = accountResult.Account.HashedId,
                PublicHashedAccountId = accountResult.Account.PublicHashedId,
                DateRegistered = accountResult.Account.CreatedDate,
                OwnerEmail = accountResult.Account.OwnerEmail,
                DasAccountName = accountResult.Account.Name,
                LegalEntities = new ResourceList(accountResult.Account.LegalEntities.Select(x => new ResourceViewModel { Id = x.ToString() })),
                PayeSchemes = new ResourceList(accountResult.Account.PayeSchemes.Select(x => new ResourceViewModel { Id = x })),
                ApprenticeshipEmployerType = accountResult.Account.ApprenticeshipEmployerType.ToString(),
                AccountAgreementType = GetAgreementType(accountResult)
            };

            return accountDetailViewModel;
        }

        private async Task<decimal> GetBalanceForAccount(long accountId)
        {
            var balanceResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = new List<long> { accountId }
            });

            var account = balanceResult?.Accounts?.SingleOrDefault();
            return account?.Balance ?? 0;
        }

        private async Task<TransferAllowance> GetTransferAllowanceForAccount(long accountId)
        {
            var transferAllowanceResult = await _mediator.SendAsync(new GetTransferAllowanceQuery
            {
                AccountId = accountId
            });

            return transferAllowanceResult.TransferAllowance;
        }

        private static AccountAgreementType GetAgreementType(GetEmployerAccountByHashedIdResponse accountResult)
        {
            var agreementTypeGroup = accountResult.Account.AccountAgreementTypes?
                .GroupBy(x => x);

            if (agreementTypeGroup == null || !agreementTypeGroup.Any())
            {
                return AccountAgreementType.Unknown;
            }

            return agreementTypeGroup?.Count() > 1 ? AccountAgreementType.Inconsistent : (AccountAgreementType)Enum.Parse(typeof(AccountAgreementType), agreementTypeGroup?.FirstOrDefault()?.Key.ToString());
        }
    }
}