using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationAuthorizationQuery, GetTransferConnectionInvitationAuthorizationResponse>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly EmployerFinanceConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public GetTransferConnectionInvitationAuthorizationQueryHandler(
            ITransferRepository transferRepository,
            EmployerFinanceConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            _transferRepository = transferRepository;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task<GetTransferConnectionInvitationAuthorizationResponse> Handle(GetTransferConnectionInvitationAuthorizationQuery message)
        {
            var authorizationResult = await _authorizationService.GetAuthorizationResultAsync("EmployerFeature.TransferConnectionRequests");
            var transferAllowance = await _transferRepository.GetTransferAllowance(message.AccountId, _configuration.TransferAllowancePercentage);

            var isValidSender = transferAllowance.RemainingTransferAllowance >= Constants.TransferConnectionInvitations.SenderMinTransferAllowance;

            return new GetTransferConnectionInvitationAuthorizationResponse
            {
                AuthorizationResult = authorizationResult,
                IsValidSender = isValidSender,
                TransferAllowancePercentage = _configuration.TransferAllowancePercentage
            };
        }
    }
}