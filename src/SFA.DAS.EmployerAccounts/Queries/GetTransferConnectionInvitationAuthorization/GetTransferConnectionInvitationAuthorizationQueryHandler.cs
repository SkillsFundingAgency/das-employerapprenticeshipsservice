using System;
using System.Data.Entity;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationAuthorizationQuery, GetTransferConnectionInvitationAuthorizationResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _accountDb;
        private readonly EmployerFinanceDbContext _financeDb;
        private readonly EmployerFinanceConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public GetTransferConnectionInvitationAuthorizationQueryHandler(
            Lazy<EmployerAccountsDbContext> accountDb,
            EmployerFinanceDbContext financeDb,
            EmployerFinanceConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            _accountDb = accountDb;
            _financeDb = financeDb;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task<GetTransferConnectionInvitationAuthorizationResponse> Handle(GetTransferConnectionInvitationAuthorizationQuery message)
        {
            var authorizationResultTask = _authorizationService.GetAuthorizationResultAsync(FeatureType.TransferConnectionRequests);
            var transferAllowanceTask = _financeDb.GetTransferAllowance(message.AccountId.Value, _configuration.TransferAllowancePercentage);

            var isReceiverTask = _accountDb.Value.TransferConnectionInvitations.AnyAsync(i =>
                i.ReceiverAccount.Id == message.AccountId && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            await Task.WhenAll(authorizationResultTask, transferAllowanceTask, isReceiverTask);

            var isValidSender = transferAllowanceTask.Result.RemainingTransferAllowance >= Constants.TransferConnectionInvitations.SenderMinTransferAllowance && !isReceiverTask.Result;

            return new GetTransferConnectionInvitationAuthorizationResponse
            {
                AuthorizationResult = authorizationResultTask.Result,
                IsValidSender = isValidSender,
                TransferAllowancePercentage = _configuration.TransferAllowancePercentage
            };
        }
    }
}