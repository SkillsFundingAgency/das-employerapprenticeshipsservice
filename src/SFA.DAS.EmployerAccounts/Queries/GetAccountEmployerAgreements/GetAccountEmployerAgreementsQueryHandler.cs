using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsQueryHandler : IRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
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

    public async Task<GetAccountEmployerAgreementsResponse> Handle(GetAccountEmployerAgreementsRequest message, CancellationToken cancellationToken)
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
            .ToListAsync(cancellationToken: cancellationToken);
                                    
        agreements = agreements.PostFixEmployerAgreementStatusDto(_hashingService, accountId).ToList();

        return new GetAccountEmployerAgreementsResponse
        {
            EmployerAgreements = agreements
        };
    }
}