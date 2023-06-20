using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountStats;

public class GetAccountStatsHandler : IRequestHandler<GetAccountStatsQuery, GetAccountStatsResponse>
{
    private readonly IEmployerAccountRepository _repository;
    private readonly IValidator<GetAccountStatsQuery> _validator;

    public GetAccountStatsHandler(IEmployerAccountRepository repository, IValidator<GetAccountStatsQuery> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<GetAccountStatsResponse> Handle(GetAccountStatsQuery message, CancellationToken cancellationToken)
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

        var stats = await _repository.GetAccountStats(message.AccountId);

        return new GetAccountStatsResponse { Stats = stats };
    }
}