﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization
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

            var isValidSender = transferAllowance.RemainingTransferAllowance >= Constants.TransferConnectionInvitations.SenderMinTransferAllowance && !isReceiver;

            return new GetTransferConnectionInvitationAuthorizationResponse
            {
                AuthorizationResult = authorizationResult,
                IsValidSender = isValidSender
            };
        }
    }
}