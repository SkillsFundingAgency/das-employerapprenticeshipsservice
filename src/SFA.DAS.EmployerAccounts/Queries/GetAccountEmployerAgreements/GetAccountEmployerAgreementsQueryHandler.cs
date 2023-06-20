using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsQueryHandler : IRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IEncodingService _encodingService;
    private readonly IValidator<GetAccountEmployerAgreementsRequest> _validator;
    private readonly IConfigurationProvider _configurationProvider;


    public GetAccountEmployerAgreementsQueryHandler(
        Lazy<EmployerAccountsDbContext> db,
        IEncodingService encodingService,
        IValidator<GetAccountEmployerAgreementsRequest> validator,
        IConfigurationProvider configurationProvider
    )
    {
        _db = db;
        _encodingService = encodingService;
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

        var agreements = await _db.Value.AccountLegalEntities
            .WithSignedOrPendingAgreementsForAccount(message.AccountId)
            .ProjectTo<EmployerAgreementStatusDto>(_configurationProvider)
            .OrderBy(ea => ea.LegalEntity.Name)
            .ToListAsync(cancellationToken: cancellationToken);
                                    
        agreements = agreements.PostFixEmployerAgreementStatusDto(_encodingService, message.AccountId).ToList();

        return new GetAccountEmployerAgreementsResponse
        {
            EmployerAgreements = agreements
        };
    }
}