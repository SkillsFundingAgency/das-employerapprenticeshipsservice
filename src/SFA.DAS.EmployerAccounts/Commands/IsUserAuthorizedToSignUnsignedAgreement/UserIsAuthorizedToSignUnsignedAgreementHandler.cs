using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Commands.IsUserAuthorizedToSignUnsignedAgreement
{
    public class UserIsAuthorizedToSignUnsignedAgreementHandler : IAsyncRequestHandler<UserIsAuthorizedToSignUnsignedAgreementCommand, UserIsAuthorizedToSignUnsignedAgreementCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public UserIsAuthorizedToSignUnsignedAgreementHandler(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<UserIsAuthorizedToSignUnsignedAgreementCommandResponse> Handle(UserIsAuthorizedToSignUnsignedAgreementCommand command)
        {
            return await UserIsAuthorizedToSignUnsignedAgreement(command.EmployerAgreement, command.EmployerAgreementRequest);
        }

        private async Task<UserIsAuthorizedToSignUnsignedAgreementCommandResponse> UserIsAuthorizedToSignUnsignedAgreement(EmployerAgreementView employerAgreement, GetEmployerAgreementRequest message)
        {
            var response = new UserIsAuthorizedToSignUnsignedAgreementCommandResponse();
            var userRef = Guid.Parse(message.ExternalUserId);
            var caller = await _accountRepository.GetMembershipUser(employerAgreement.AccountId, userRef);

            if (caller == null)
            {
                response.IsAuthorized = false;
            }

            if (employerAgreement.HashedAccountId != message.HashedAccountId || (employerAgreement.Status != EmployerAgreementStatus.Signed && caller?.Role != Role.Owner))
            {
                response.IsAuthorized = false;
            }
            response.IsAuthorized = true;
            return response;
        }
    }
}