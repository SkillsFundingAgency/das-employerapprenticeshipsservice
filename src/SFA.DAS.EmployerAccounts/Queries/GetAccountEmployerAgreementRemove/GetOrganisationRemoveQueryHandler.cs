using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove
{
    public class GetOrganisationRemoveQueryHandler : IAsyncRequestHandler<GetOrganisationRemoveRequest, GetOrganisationRemoveResponse>
    {
        private readonly IValidator<GetOrganisationRemoveRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;

        public GetOrganisationRemoveQueryHandler(
            IValidator<GetOrganisationRemoveRequest> validator, 
            IEmployerAgreementRepository employerAgreementRepository, 
            IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
        }

        public async Task<GetOrganisationRemoveResponse> Handle(GetOrganisationRemoveRequest message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }
            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var accountLegalEntityId = _accountLegalEntityPublicHashingService.DecodeValue(message.AccountLegalEntityPublicHashedId);

            var accountLegalEntity = await _employerAgreementRepository.GetAccountLegalEntity(accountLegalEntityId);

            if (accountLegalEntity == null)
            {
                return new GetOrganisationRemoveResponse();
            }

            var agreementView = new RemoveOrganisationView
            {
                CanBeRemoved = true,
                HashedAccountId = message.HashedAccountId,
                AccountLegalEntityPublicHashedId = message.AccountLegalEntityPublicHashedId,
                AccountLegalEntityId = accountLegalEntityId,
                Name = accountLegalEntity.Name,
                HasSignedAgreement = accountLegalEntity.SignedAgreementId.HasValue 
            };

            return new GetOrganisationRemoveResponse {Organisation = agreementView};
        }
    }
}
