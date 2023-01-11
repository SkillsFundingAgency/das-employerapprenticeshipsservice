using System.Data.Entity;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest,
    GetAccountEmployerAgreementsResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IHashingService _hashingService;
    private readonly IValidator<GetAccountEmployerAgreementsRequest> _validator;
    private readonly IConfigurationProvider _configurationProvider;


    public GetAccountEmployerAgreementsQueryHandler(
        Lazy<EmployerAccountsDbContext> db,
        IHashingService hashingService,
        IValidator<GetAccountEmployerAgreementsRequest> validator,
        IConfigurationProvider configurationProvider
    )
    {
        _db = db;
        _hashingService = hashingService;
        _validator = validator;
        _configurationProvider = configurationProvider;
    }

    public async Task<GetAccountEmployerAgreementsResponse> Handle(GetAccountEmployerAgreementsRequest message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accountId = _hashingService.DecodeValue(message.HashedAccountId);

        var agreements = await _db.Value.AccountLegalEntities
            .WithSignedOrPendingAgreementsForAccount(accountId)
            .ProjectTo<EmployerAgreementStatusDto>(_configurationProvider)
            .OrderBy(ea => ea.LegalEntity.Name)
            .ToListAsync();
                                    
        agreements = agreements.PostFixEmployerAgreementStatusDto(_hashingService, accountId).ToList();

        return new GetAccountEmployerAgreementsResponse
        {
            EmployerAgreements = agreements
        };
    }
}