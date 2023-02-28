using System.Threading;
using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsQueryHandler : IRequestHandler<GetOrganisationAgreementsRequest, GetOrganisationAgreementsResponse>
{
    private readonly IValidator<GetOrganisationAgreementsRequest> _validator;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IEncodingService _encodingService;
    private readonly IReferenceDataService _referenceDataService;
    private readonly IMapper _mapper;

    public GetOrganisationAgreementsQueryHandler(IValidator<GetOrganisationAgreementsRequest> validator,
        IEmployerAgreementRepository employerAgreementRepository,
        IEncodingService encodingService,
        IReferenceDataService referenceDataService,
        IMapper mapper)
    {
        _validator = validator;
        _employerAgreementRepository = employerAgreementRepository;
        _encodingService = encodingService;
        _referenceDataService = referenceDataService;
        _mapper = mapper;
    }


    public async Task<GetOrganisationAgreementsResponse> Handle(GetOrganisationAgreementsRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accountLegalEntityId = _encodingService.Decode(message.AccountLegalEntityHashedId, EncodingType.PublicAccountLegalEntityId);

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
                        e.HashedAccountId = _encodingService.Encode(accountLegalEntity.AccountId, EncodingType.AccountId);
                        e.HashedAgreementId = _encodingService.Encode(e.Id, EncodingType.AccountId);
                        e.HashedLegalEntityId = _encodingService.Encode(accountLegalEntity.Id, EncodingType.AccountId);
                        e.OrganisationLookupPossible = organisationLookupByIdPossible || accountLegalEntity.LegalEntity.Source == OrganisationType.PensionsRegulator;
                    });
                });
            });

        return new GetOrganisationAgreementsResponse { Agreements = agreements.OrderByDescending(a => a.TemplateId).ToList() };
    }
}