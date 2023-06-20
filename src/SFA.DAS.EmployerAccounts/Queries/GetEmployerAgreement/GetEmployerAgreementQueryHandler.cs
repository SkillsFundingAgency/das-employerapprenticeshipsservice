using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;

public class GetEmployerAgreementQueryHandler : IRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _database;
    private readonly IEncodingService _encodingService;
    private readonly IValidator<GetEmployerAgreementRequest> _validator;
    private readonly IConfigurationProvider _configurationProvider;
        
    public GetEmployerAgreementQueryHandler(
        Lazy<EmployerAccountsDbContext> database,
        IEncodingService encodingService,
        IValidator<GetEmployerAgreementRequest> validator,
        IConfigurationProvider configurationProvider)
    {
        _database = database;
        _encodingService = encodingService;
        _validator = validator;
        _configurationProvider = configurationProvider;
    }

    public async Task<GetEmployerAgreementResponse> Handle(GetEmployerAgreementRequest message, CancellationToken cancellationToken)
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

        var agreementId = _encodingService.Decode(message.HashedAgreementId, EncodingType.AccountId);
        var employerAgreement = await _database.Value.Agreements.ProjectTo<AgreementDto>(_configurationProvider)
            .SingleOrDefaultAsync(x => x.Id.Equals(agreementId), cancellationToken);

        if (employerAgreement == null)
            return new GetEmployerAgreementResponse();

        employerAgreement.HashedAccountId = message.HashedAccountId;
        employerAgreement.HashedAgreementId = message.HashedAgreementId;

        if (employerAgreement.StatusId != EmployerAgreementStatus.Signed)
        {
            employerAgreement.SignedByName = GetUserFullName(message.ExternalUserId);
        }

        return new GetEmployerAgreementResponse
        {
            EmployerAgreement = employerAgreement
        };
    }

    private string GetUserFullName(string userId)
    {
        var externalUserId = Guid.Parse(userId);
        var user = _database.Value.Users
            .Where(m => m.Ref == externalUserId)
            .Select(m => m)
            .Single();
        return user.FullName;
    }
}