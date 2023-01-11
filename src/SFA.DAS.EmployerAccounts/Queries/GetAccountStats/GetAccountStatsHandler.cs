using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountStats;

public class GetAccountStatsHandler : IAsyncRequestHandler<GetAccountStatsQuery, GetAccountStatsResponse>
{
    private readonly IEmployerAccountRepository _repository;
    private readonly IHashingService _hashingService;
    private readonly IValidator<GetAccountStatsQuery> _validator;

    public GetAccountStatsHandler(IEmployerAccountRepository repository, IHashingService hashingService, IValidator<GetAccountStatsQuery> validator)
    {
        _repository = repository;
        _hashingService = hashingService;
        _validator = validator;
    }

    public async Task<GetAccountStatsResponse> Handle(GetAccountStatsQuery message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException("User not authorised");
            }

            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accoundId = _hashingService.DecodeValue(message.HashedAccountId);

        var stats = await _repository.GetAccountStats(accoundId);

        return new GetAccountStatsResponse { Stats = stats };
    }
}