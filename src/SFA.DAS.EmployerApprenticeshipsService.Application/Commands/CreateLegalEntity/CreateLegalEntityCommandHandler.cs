using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;

        public CreateLegalEntityCommandHandler(IAccountRepository accountRepository, IMembershipRepository membershipRepository, IMediator mediator)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
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

            await _mediator.SendAsync(new CreateAccountEventCommand { HashedAccountId = message.HashedAccountId, Event = "LegalEntityCreated" });
            
            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }
    }
}
