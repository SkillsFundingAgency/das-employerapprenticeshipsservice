using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementQueryHandler : IRequestHandler<GetLastSignedAgreementRequest, GetLastSignedAgreementResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _database;
    private readonly IEncodingService __encodingService;
    private readonly IValidator<GetLastSignedAgreementRequest> _validator;
    private readonly IConfigurationProvider _configurationProvider;

    public GetLastSignedAgreementQueryHandler(
        Lazy<EmployerAccountsDbContext> database,
        IEncodingService encodingService,
        IValidator<GetLastSignedAgreementRequest> validator,
        IConfigurationProvider configurationProvider)
    {
        _database = database;
        __encodingService = encodingService;
        _validator = validator;
        _configurationProvider = configurationProvider;
    }

    public async Task<GetLastSignedAgreementResponse> Handle(GetLastSignedAgreementRequest message, CancellationToken cancellationToken)
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
            lastSignedAgreement.HashedAccountId = __encodingService.Encode(lastSignedAgreement.AccountId, EncodingType.AccountId);
            lastSignedAgreement.HashedAgreementId = __encodingService.Encode(lastSignedAgreement.Id, EncodingType.AccountId);
        }

        return new GetLastSignedAgreementResponse
        {
            LastSignedAgreement = lastSignedAgreement
        };
    }
}