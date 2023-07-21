using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;

public class GetAccountLegalEntitiesQueryHandler : IRequestHandler<GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IValidator<GetAccountLegalEntitiesRequest> _validator;

    public GetAccountLegalEntitiesQueryHandler(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IValidator<GetAccountLegalEntitiesRequest> validator)
    {
        _membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        _employerAgreementRepository = employerAgreementRepository ?? throw new ArgumentNullException(nameof(employerAgreementRepository));
        _validator = validator;
    }

    public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesRequest message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var membership = await _membershipRepository.GetCaller(message.HashedLegalEntityId, message.UserId);

        if (membership == null)
            throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not a member of this account" } });

        var accountSpecificLegalEntity = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(membership.AccountId, false);

        return new GetAccountLegalEntitiesResponse
        {
            LegalEntities = accountSpecificLegalEntity.ToList()
        };
    }
}