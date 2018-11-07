using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
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

        public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(string hashedAccountId)
        {
            _logger.Info($"Getting account {hashedAccountId}");

            var accountResult = await _mediator.SendAsync(new GetEmployerAccountByHashedIdQuery { HashedAccountId = hashedAccountId });
            if (accountResult.Account == null)
            {
                return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
            }

            var viewModel = ConvertAccountDetailToViewModel(accountResult);

            // TODO THE BALANCE NEEDS TO BE RETRIEVED ONLY IN CALLS THAT USE IT
            //var accountBalanceTask = GetBalanceForAccount(accountResult.Account.AccountId);
            //var transferBalanceTask = GetTransferAllowanceForAccount(accountResult.Account.AccountId);

            //await Task.WhenAll(accountBalanceTask, transferBalanceTask).ConfigureAwait(false);

            //viewModel.Balance = accountBalanceTask.Result;
            //viewModel.RemainingTransferAllowance = transferBalanceTask.Result.RemainingTransferAllowance ?? 0;
            //viewModel.StartingTransferAllowance = transferBalanceTask.Result.StartingTransferAllowance ?? 0;

            return new OrchestratorResponse<AccountDetailViewModel> { Data = viewModel };
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
                PayeSchemes = new ResourceList(accountResult.Account.PayeSchemes.Select(x => new ResourceViewModel { Id = x }))
            };

            return accountDetailViewModel;
        }

    }
}