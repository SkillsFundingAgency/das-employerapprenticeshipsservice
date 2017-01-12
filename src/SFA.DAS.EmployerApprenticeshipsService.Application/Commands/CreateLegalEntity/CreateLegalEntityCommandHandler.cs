using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEventsApi _eventsApi;

        public CreateLegalEntityCommandHandler(IAccountRepository accountRepository, IMembershipRepository membershipRepository, IEventsApi eventsApi)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _eventsApi = eventsApi;
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

            await _eventsApi.CreateAccountEvent(new AccountEvent { EmployerAccountId = message.HashedAccountId, Event = "LegalEntityCreated" });
            
            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }
    }
}
