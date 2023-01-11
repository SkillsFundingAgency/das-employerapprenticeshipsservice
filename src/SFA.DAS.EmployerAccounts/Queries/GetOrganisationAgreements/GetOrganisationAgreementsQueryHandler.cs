using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsQueryHandler : IAsyncRequestHandler<GetOrganisationAgreementsRequest, GetOrganisationAgreementsResponse>
{
    private readonly IValidator<GetOrganisationAgreementsRequest> _validator;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
    private readonly IHashingService _hashingService;
    private readonly IReferenceDataService _referenceDataService;
    private readonly IMapper _mapper;

    public GetOrganisationAgreementsQueryHandler(IValidator<GetOrganisationAgreementsRequest> validator,
        IEmployerAgreementRepository employerAgreementRepository,
        IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
        IHashingService hashingService,
        IReferenceDataService referenceDataService,
        IMapper mapper)
    {
        _validator = validator;
        _employerAgreementRepository = employerAgreementRepository;
        _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
        _hashingService = hashingService;
        _referenceDataService = referenceDataService;
        _mapper = mapper;
    }


    public async Task<GetOrganisationAgreementsResponse> Handle(GetOrganisationAgreementsRequest message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accountLegalEntityId = _accountLegalEntityPublicHashingService.DecodeValue(message.AccountLegalEntityHashedId);

        var accountLegalEntity = await _employerAgreementRepository.GetOrganisationsAgreements(accountLegalEntityId);          
        if (accountLegalEntity == null) return new GetOrganisationAgreementsResponse();

        var organisationLookupByIdPossible = await _referenceDataService.IsIdentifiableOrganisationType(accountLegalEntity.LegalEntity.Source);

        var employerAgreements = accountLegalEntity.Agreements.Where(x => x.StatusId == EmployerAgreementStatus.Pending || x.SignedDate.HasValue).ToList();

        var agreements = _mapper.Map<ICollection<EmployerAgreement>, ICollection<EmployerAgreementDto>>
        (employerAgreements,
            opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    dest.ToList().ForEach(e =>
                    {
                        e.HashedAccountId = _hashingService.HashValue(accountLegalEntity.AccountId);
                        e.HashedAgreementId = _hashingService.HashValue(e.Id);
                        e.HashedLegalEntityId = _hashingService.HashValue(accountLegalEntity.Id);
                        e.OrganisationLookupPossible = organisationLookupByIdPossible || accountLegalEntity.LegalEntity.Source == OrganisationType.PensionsRegulator;
                    });
                });
            });

        return new GetOrganisationAgreementsResponse { Agreements = agreements.OrderByDescending(a => a.TemplateId).ToList() };
    }
}