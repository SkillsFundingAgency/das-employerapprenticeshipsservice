using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements
{
    public class GetOrganisationAgreementsQueryHandler : IAsyncRequestHandler<GetOrganisationAgreementsRequest, GetOrganisationAgreementsResponse>
    {
        private readonly IValidator<GetOrganisationAgreementsRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
        private readonly IHashingService _hashingService;
        private readonly IReferenceDataService _referenceDataService;

        public GetOrganisationAgreementsQueryHandler(IValidator<GetOrganisationAgreementsRequest> validator,
           IEmployerAgreementRepository employerAgreementRepository,
           IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
           IHashingService hashingService,
           IReferenceDataService referenceDataService)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
            _hashingService = hashingService;
            _referenceDataService = referenceDataService;
        }

        public async Task<GetOrganisationAgreementsResponse> Handle(GetOrganisationAgreementsRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountLegalEntityId = _accountLegalEntityPublicHashingService.DecodeValue(message.AccountLegalEntityHashedId);

            var result = await _employerAgreementRepository.GetOrganisationAgreement(accountLegalEntityId);
            if (result == null) return new GetOrganisationAgreementsResponse();

            var organisationLookupByIdPossible = await _referenceDataService.IsIdentifiableOrganisationType(result.LegalEntity.Source);

            foreach (var agreement in result.Agreements)
            {

                switch (agreement.Template.PartialViewName)
                {
                    case "_Agreement_V3":
                        OrganisationAgreement agreementV3 = GetAgreement(result, agreement, organisationLookupByIdPossible);
                        result.EmployerAgreementV3 = agreementV3.EmployerAgreementV2;
                        break;
                    case "_Agreement_V2":
                        OrganisationAgreement agreementV2 = GetAgreement(result, agreement, organisationLookupByIdPossible);
                        result.EmployerAgreementV2 = agreementV2.EmployerAgreementV2;
                        break;
                    case "_Agreement_V1":
                        OrganisationAgreement agreementV1 = GetAgreement(result, agreement, organisationLookupByIdPossible);
                        result.EmployerAgreementV1 = agreementV1.EmployerAgreementV1;
                        break;
                }

            }

            result.HashedAccountId = _hashingService.HashValue(result.AccountId); //AccountId            
            result.HashedLegalEntityId = _hashingService.HashValue(result.Id); //AccountLegalEntityId

            return new GetOrganisationAgreementsResponse { OrganisationAgreements = result };
        }

        private OrganisationAgreement GetAgreement(OrganisationAgreement result, EmployerAgreement agreement, bool organisationLookupByIdPossible)
        {
            return new OrganisationAgreement()
            {
                EmployerAgreementV2 = new EmployerAgreementDto
                {
                    SignedByName = agreement.SignedByName,
                    SignedDate = agreement.SignedDate,
                    AccountLegalEntity = new AccountLegalEntity
                    {
                        Id = result.Id,
                        AccountId = result.AccountId,
                        Name = result.Name,
                        Address = result.Address,
                        PublicHashedId = result.AccountLegalEntityPublicHashedId

                    },
                    HashedAgreementId = _hashingService.HashValue(agreement.Id),
                    HashedAccountId = _hashingService.HashValue(result.AccountId),
                    HashedLegalEntityId = _hashingService.HashValue(result.Id),
                    OrganisationLookupPossible = organisationLookupByIdPossible
                }
            };
        }
    }
}