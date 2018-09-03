using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationAuthorizationQuery, GetTransferConnectionInvitationAuthorizationResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _accountDb;
        private readonly EmployerFinanceDbContext _financeDb;
        private readonly LevyDeclarationProviderConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public GetTransferConnectionInvitationAuthorizationQueryHandler(
            Lazy<EmployerAccountsDbContext> accountDb,
            EmployerFinanceDbContext financeDb,
            LevyDeclarationProviderConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            _accountDb = accountDb;
            _financeDb = financeDb;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task<GetTransferConnectionInvitationAuthorizationResponse> Handle(GetTransferConnectionInvitationAuthorizationQuery message)
        {
            var authorizationResult = await _authorizationService.GetAuthorizationResultAsync(FeatureType.TransferConnectionRequests);
            var transferAllowance = await _financeDb.GetTransferAllowance(message.AccountId.Value, _configuration.TransferAllowancePercentage);

            var isReceiver = await _accountDb.Value.TransferConnectionInvitations.AnyAsync(i =>
                i.ReceiverAccount.Id == message.AccountId && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            var sentCount = await _accountDb.Value.TransferConnectionInvitations.CountAsync(i =>
                i.SenderAccount.Id == message.AccountId && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            var isValidSender = transferAllowance >= Constants.TransferConnectionInvitations.SenderMinTransferAllowance && !isReceiver;

            return new GetTransferConnectionInvitationAuthorizationResponse
            {
                AuthorizationResult = authorizationResult,
                IsValidSender = isValidSender
            };
        }
    }
}