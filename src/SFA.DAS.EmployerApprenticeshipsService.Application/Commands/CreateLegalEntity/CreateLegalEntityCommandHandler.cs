using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;

        public CreateLegalEntityCommandHandler(IAccountRepository accountRepository, IMembershipRepository membershipRepository)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<CreateLegalEntityCommandResponse> Handle(CreateLegalEntityCommand message)
        {
            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            var agreementView = await _accountRepository.CreateLegalEntity(
                owner.AccountId, 
                message.LegalEntity, 
                message.SignAgreement, 
                message.SignedDate,
                owner.UserId);
            
            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }
    }
}
