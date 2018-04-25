﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityById;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclarationsByAccountAndPeriod;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Application.Queries.GetTeamMembers;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;

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

            var accountsResult = await _mediator.SendAsync(new GetPagedEmployerAccountsQuery() { ToDate = toDate, PageSize = pageSize, PageNumber = pageNumber });
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
                    IsLevyPayer = true
                };
                
                if (accountBalanceHash.TryGetValue(account.Id, out var accountBalance))
                {
                    accountBalanceModel.Balance = accountBalance.Balance;
                    accountBalanceModel.TransferAllowance = accountBalance.TransferAllowance;
                    accountBalanceModel.IsLevyPayer = accountBalance.IsLevyPayer == 1;
                }

                data.Add(accountBalanceModel);
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>() { Data = new PagedApiResponseViewModel<AccountWithBalanceViewModel>() { Data = data, Page = pageNumber, TotalPages = (accountsResult.AccountsCount / pageSize) + 1 } };
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

            var tasks = new []
            {
                GetBalanceForAccount(accountResult.Account.AccountId),
                GetTransferAllowanceForAccount(accountResult.Account.AccountId)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            viewModel.Balance = tasks[0].Result;
            viewModel.TransferAllowance = tasks[1].Result;

            return new OrchestratorResponse<AccountDetailViewModel> { Data = viewModel };
        }

        public async Task<OrchestratorResponse<LegalEntityViewModel>> GetLegalEntity(string hashedAccountId, long legalEntityId)
        {
            _logger.Info($"Getting legal entity {legalEntityId} for account {hashedAccountId}");
            var legalEntityResult = await _mediator.SendAsync(new GetLegalEntityByIdQuery { HashedAccountId = hashedAccountId, Id = legalEntityId });
            if (legalEntityResult.LegalEntity == null)
            {
                return new OrchestratorResponse<LegalEntityViewModel> { Data = null };
            }

            var viewModel = ConvertLegalEntityToViewModel(hashedAccountId, legalEntityResult);
            return new OrchestratorResponse<LegalEntityViewModel> { Data = viewModel };
        }

        public async Task<OrchestratorResponse<PayeSchemeViewModel>> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            _logger.Info($"Getting paye scheme {payeSchemeRef} for account {hashedAccountId}");

            var payeSchemeResult = await _mediator.SendAsync(new GetPayeSchemeByRefQuery { HashedAccountId = hashedAccountId, Ref = payeSchemeRef });
            if (payeSchemeResult.PayeScheme == null)
            {
                return new OrchestratorResponse<PayeSchemeViewModel> { Data = null };
            }

            var viewModel = ConvertPayeSchemeToViewModel(hashedAccountId, payeSchemeResult);
            return new OrchestratorResponse<PayeSchemeViewModel> { Data = viewModel };
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

            var teamMembers = await _mediator.SendAsync(new GetTeamMembersRequest {HashedAccountId = hashedAccountId});

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

            var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = hashedAccountId, PayrollYear = payrollYear, PayrollMonth = payrollMonth});
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

        private LegalEntityViewModel ConvertLegalEntityToViewModel(string hashedAccountId, GetLegalEntityByIdResponse legalEntityResult)
        {
            var legalEntityViewModel = new LegalEntityViewModel
            {
                DasAccountId = hashedAccountId,
                DateOfInception = legalEntityResult.LegalEntity.DateOfInception,
                LegalEntityId = legalEntityResult.LegalEntity.Id,
                Name = legalEntityResult.LegalEntity.Name,
                Source = legalEntityResult.LegalEntity.Source,
                PublicSectorDataSource = legalEntityResult.LegalEntity.PublicSectorDataSource,
                Address = legalEntityResult.LegalEntity.Address,
                Code = legalEntityResult.LegalEntity.Code,
                Status = legalEntityResult.LegalEntity.Status,
                Sector = legalEntityResult.LegalEntity.Sector,
                SourceNumeric = legalEntityResult.LegalEntity.SourceNumeric,
                AgreementStatus = (EmployerAgreementStatus)((int)legalEntityResult.LegalEntity.AgreementStatus),
                AgreementSignedByName = legalEntityResult.LegalEntity.AgreementSignedByName,
                AgreementSignedDate = legalEntityResult.LegalEntity.AgreementSignedDate
            };

            return legalEntityViewModel;
        }

        private static AccountDetailViewModel ConvertAccountDetailToViewModel(GetEmployerAccountByHashedIdResponse accountResult)
        {
            var accountDetailViewModel = new AccountDetailViewModel
            {
                AccountId = accountResult.Account.AccountId,
                HashedAccountId = accountResult.Account.HashedId,
                DateRegistered = accountResult.Account.CreatedDate,
                OwnerEmail = accountResult.Account.OwnerEmail,
                DasAccountName = accountResult.Account.Name,
                LegalEntities = new ResourceList(accountResult.Account.LegalEntities.Select(x => new ResourceViewModel { Id = x.ToString() })),
                PayeSchemes = new ResourceList(accountResult.Account.PayeSchemes.Select(x => new ResourceViewModel { Id = x }))
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

        private async Task<decimal> GetTransferAllowanceForAccount(long accountId)
        {
            var transferAllowanceResult = await _mediator.SendAsync(new GetAccountTransferAllowanceRequest
            {
                AccountId = accountId
            });

            return transferAllowanceResult?.TransferAllowance ?? 0M;
        }
    }
}