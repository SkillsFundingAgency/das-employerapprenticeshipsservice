using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;

public class GetAccountLegalEntityRemoveQueryHandler : IRequestHandler<GetAccountLegalEntityRemoveRequest, GetAccountLegalEntityRemoveResponse>
{
    private readonly IValidator<GetAccountLegalEntityRemoveRequest> _validator;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IEncodingService _encodingService;
    private readonly ICommitmentsV2ApiClient _commitmentV2ApiClient;

    public GetAccountLegalEntityRemoveQueryHandler(
        IValidator<GetAccountLegalEntityRemoveRequest> validator,
        IEmployerAgreementRepository employerAgreementRepository, 
        IEncodingService encodingService,
        ICommitmentsV2ApiClient commitmentV2ApiClient)
    {
        _validator = validator;
        _employerAgreementRepository = employerAgreementRepository;
        _encodingService = encodingService;
        _commitmentV2ApiClient = commitmentV2ApiClient;
    }

    public async Task<GetAccountLegalEntityRemoveResponse> Handle(GetAccountLegalEntityRemoveRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        if (validationResult.IsUnauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        var accountId = _encodingService.Decode(message.HashedAccountId, EncodingType.AccountId);
        var accountLegalEntityId = _encodingService.Decode(message.HashedAccountLegalEntityId, EncodingType.PublicAccountLegalEntityId);
        var accountLegalEntity = await _employerAgreementRepository.GetAccountLegalEntity(accountLegalEntityId);

        var result = await _employerAgreementRepository.GetAccountLegalEntityAgreements(accountLegalEntityId);
        if (result == null) return new GetAccountLegalEntityRemoveResponse();

        if (result.Any(x => x.SignedDate.HasValue))
        {
            return new GetAccountLegalEntityRemoveResponse
            {
                CanBeRemoved = await SetRemovedStatusBasedOnCommitments(accountId, accountLegalEntity),
                HasSignedAgreement = true,
                Name = accountLegalEntity.Name
            };
        }

        return new GetAccountLegalEntityRemoveResponse
        {
            CanBeRemoved = true,
            HasSignedAgreement = false,
            Name = accountLegalEntity.Name
        };
    }

    private async Task<bool> SetRemovedStatusBasedOnCommitments(long accountId, AccountLegalEntityModel accountLegalEntityModel)
    {
        var commitments = await _commitmentV2ApiClient.GetEmployerAccountSummary(accountId);

        var commitmentConnectedToEntity = commitments.ApprenticeshipStatusSummaryResponse.FirstOrDefault(c =>
            !string.IsNullOrEmpty(c.LegalEntityIdentifier)
            && c.LegalEntityIdentifier.Equals(accountLegalEntityModel.Identifier)
            && c.LegalEntityOrganisationType == accountLegalEntityModel.OrganisationType);

        return commitmentConnectedToEntity == null || (commitmentConnectedToEntity.ActiveCount +
                                                       commitmentConnectedToEntity.PendingApprovalCount +
                                                       commitmentConnectedToEntity.WithdrawnCount +
                                                       commitmentConnectedToEntity.PausedCount) == 0;
    }
}