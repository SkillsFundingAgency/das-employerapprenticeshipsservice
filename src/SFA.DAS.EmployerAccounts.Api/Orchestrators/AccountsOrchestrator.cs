using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;
using SFA.DAS.Encoding;
using PayeScheme = SFA.DAS.EmployerAccounts.Api.Types.PayeScheme;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsOrchestrator> _logger;
        private readonly IMapper _mapper;
        private readonly IEncodingService _encodingService;

        public AccountsOrchestrator(
            IMediator mediator,
            ILogger<AccountsOrchestrator> logger,
            IMapper mapper,
            IEncodingService encodingService)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _encodingService = encodingService;
        }

        public async Task<PayeScheme> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            _logger.LogInformation("Getting paye scheme {PayeSchemeRef} for account {HashedAccountId}", payeSchemeRef, hashedAccountId);

            var payeSchemeResult = await _mediator.Send(new GetPayeSchemeByRefQuery { HashedAccountId = hashedAccountId, Ref = payeSchemeRef });
            return payeSchemeResult.PayeScheme == null ? null : ConvertToPayeScheme(hashedAccountId, payeSchemeResult);
        }

        public async Task<AccountDetail> GetAccount(string hashedAccountId)
        {
            _logger.LogInformation("Getting account {HashedAccountId}", hashedAccountId);

            var accountResult = await _mediator.Send(new GetEmployerAccountDetailByHashedIdQuery { HashedAccountId = hashedAccountId });
            return accountResult.Account == null ? null : ConvertToAccountDetail(accountResult);
        }

        public async Task<PagedApiResponse<Account>> GetAccounts(string toDate, int pageSize, int pageNumber)
        {
            _logger.LogInformation("Getting all accounts.");

            toDate = toDate ?? DateTime.MaxValue.ToString("yyyyMMddHHmmss");

            var accountsResult = await _mediator.Send(new GetPagedEmployerAccountsQuery { ToDate = toDate, PageSize = pageSize, PageNumber = pageNumber });

            var data = new List<Account>();

            accountsResult.Accounts.ForEach(account =>
            {
                var accountModel = new Account
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    AccountHashId = account.HashedId,
                    PublicAccountHashId = account.PublicHashedId,
                    ApprenticeshipEmployerType = (ApprenticeshipEmployerType)account.ApprenticeshipEmployerType,
                    AccountAgreementType = GetAgreementType(account.AccountLegalEntities.SelectMany(x => x.Agreements.Where(y => y.SignedDate.HasValue)).Select(x => x.Template.AgreementType).Distinct().ToList())
                };

                data.Add(accountModel);
            });

            return new PagedApiResponse<Account>
            {
                Data = data,
                Page = pageNumber,
                TotalPages = (accountsResult.AccountsCount / pageSize) + 1
            };
        }

        public async Task<List<TeamMember>> GetAccountTeamMembers(long accountId)
        {
            _logger.LogInformation("Requesting team members for account {AccountId}", accountId);

            var teamMembers = await _mediator.Send(new GetTeamMembersRequest(accountId));
            return teamMembers.TeamMembers.Select(x => _mapper.Map<TeamMember>(x)).ToList();
        }

        public async Task<List<TeamMember>> GetAccountTeamMembersWhichReceiveNotifications(long accountId)
        {
            _logger.LogInformation("Requesting team members which receive notifications for account {AccountId}", accountId);

            var teamMembers = await _mediator.Send(new GetTeamMembersWhichReceiveNotificationsQuery { AccountId = accountId });
            return teamMembers.TeamMembersWhichReceiveNotifications.Select(x => _mapper.Map<TeamMember>(x)).ToList();
        }

        public async Task<IEnumerable<PayeView>> GetPayeSchemesForAccount(string hashedAccountId)
        {
            try
            {
                var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
                var response = await _mediator.Send(new GetAccountPayeSchemesQuery { AccountId = accountId });

                return response.PayeSchemes;
            }
            catch (InvalidRequestException)
            {
                return null;
            }
        }

        private static PayeScheme ConvertToPayeScheme(string hashedAccountId, GetPayeSchemeByRefResponse payeSchemeResult)
        {
            return new PayeScheme
            {
                DasAccountId = hashedAccountId,
                Name = payeSchemeResult.PayeScheme.Name,
                Ref = payeSchemeResult.PayeScheme.Ref,
                AddedDate = payeSchemeResult.PayeScheme.AddedDate,
                RemovedDate = payeSchemeResult.PayeScheme.RemovedDate
            };
        }

        private static AccountDetail ConvertToAccountDetail(GetEmployerAccountDetailByHashedIdResponse accountResult)
        {
            return new AccountDetail
            {
                AccountId = accountResult.Account.AccountId,
                HashedAccountId = accountResult.Account.HashedId,
                PublicHashedAccountId = accountResult.Account.PublicHashedId,
                DateRegistered = accountResult.Account.CreatedDate,
                OwnerEmail = accountResult.Account.OwnerEmail,
                DasAccountName = accountResult.Account.Name,
                LegalEntities = new ResourceList(accountResult.Account.LegalEntities.Select(x => new Resource { Id = x.ToString() })),
                PayeSchemes = new ResourceList(accountResult.Account.PayeSchemes.Select(x => new Resource { Id = x })),
                ApprenticeshipEmployerType = accountResult.Account.ApprenticeshipEmployerType.ToString(),
                AccountAgreementType = GetAgreementType(accountResult.Account.AccountAgreementTypes)
            };
        }

        private static AccountAgreementType GetAgreementType(List<AgreementType> agreementTypes)
        {
            var agreementTypeGroup = agreementTypes?.GroupBy(x => x).OrderByDescending(x => x.Key);

            if (agreementTypeGroup == null || !agreementTypeGroup.Any())
            {
                return AccountAgreementType.Unknown;
            }

            return (AccountAgreementType)Enum.Parse(typeof(AccountAgreementType), agreementTypeGroup?.FirstOrDefault()?.Key.ToString());
        }
    }
}