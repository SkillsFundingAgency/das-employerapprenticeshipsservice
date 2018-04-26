using System.Data.Entity;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationAuthorizationQuery, GetTransferConnectionInvitationAuthorizationResponse>
    {
        private readonly EmployerAccountDbContext _accountDb;
        private readonly EmployerFinancialDbContext _financialDb;
        private readonly LevyDeclarationProviderConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public GetTransferConnectionInvitationAuthorizationQueryHandler(
            EmployerAccountDbContext accountDb,
            EmployerFinancialDbContext financialDb,
            LevyDeclarationProviderConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            _accountDb = accountDb;
            _financialDb = financialDb;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task<GetTransferConnectionInvitationAuthorizationResponse> Handle(GetTransferConnectionInvitationAuthorizationQuery message)
        {
            var authorizationResult = await _authorizationService.GetAuthorizationResultAsync(FeatureType.TransferConnectionRequests);
            var transferAllowance = await _financialDb.GetTransferAllowance(message.AccountId.Value, _configuration.TransferAllowancePercentage);

            var isReceiver = await _accountDb.TransferConnectionInvitations.AnyAsync(i =>
                i.ReceiverAccount.Id == message.AccountId && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            var sentCount = await _accountDb.TransferConnectionInvitations.CountAsync(i =>
                i.SenderAccount.Id == message.AccountId && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            var isValidSender = 
                transferAllowance >= Constants.TransferConnectionInvitations.SenderMinTransferAllowance &&
                !isReceiver &&
                sentCount < Constants.TransferConnectionInvitations.SenderMaxTransferConnectionInvitations;

            return new GetTransferConnectionInvitationAuthorizationResponse
            {
                AuthorizationResult = authorizationResult,
                IsValidSender = isValidSender
            };
        }
    }
}