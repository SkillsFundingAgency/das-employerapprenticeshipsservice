using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementQueryHandler : IAsyncRequestHandler<GetLastSignedAgreementRequest, GetLastSignedAgreementResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _database;
    private readonly IHashingService _hashingService;
    private readonly IValidator<GetLastSignedAgreementRequest> _validator;
    private readonly IConfigurationProvider _configurationProvider;

    public GetLastSignedAgreementQueryHandler(
        Lazy<EmployerAccountsDbContext> database,
        IHashingService hashingService,
        IValidator<GetLastSignedAgreementRequest> validator,
        IConfigurationProvider configurationProvider)
    {
        _database = database;
        _hashingService = hashingService;
        _validator = validator;
        _configurationProvider = configurationProvider;
    }

    public async Task<GetLastSignedAgreementResponse> Handle(GetLastSignedAgreementRequest message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var lastSignedAgreement = _database.Value.Agreements
            .Where(x => x.AccountLegalEntityId == message.AccountLegalEntityId && x.StatusId == EmployerAgreementStatus.Signed)
            .OrderByDescending(x => x.TemplateId)
            .ProjectTo<AgreementDto>(_configurationProvider)
            .FirstOrDefault();

        if (lastSignedAgreement != null)
        {
            lastSignedAgreement.HashedAccountId = _hashingService.HashValue(lastSignedAgreement.AccountId);
            lastSignedAgreement.HashedAgreementId = _hashingService.HashValue(lastSignedAgreement.Id);
            lastSignedAgreement.HashedLegalEntityId = _hashingService.HashValue(lastSignedAgreement.LegalEntityId);
        }

        return new GetLastSignedAgreementResponse
        {
            LastSignedAgreement = lastSignedAgreement
        };
    }
}