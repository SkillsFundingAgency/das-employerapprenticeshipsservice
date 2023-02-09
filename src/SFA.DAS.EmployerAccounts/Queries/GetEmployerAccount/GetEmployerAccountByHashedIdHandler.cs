using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByHashedIdHandler : IRequestHandler<GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>
{
    private readonly IEmployerAccountRepository _employerAccountRepository;
    private readonly IValidator<GetEmployerAccountByHashedIdQuery> _validator;

    public GetEmployerAccountByHashedIdHandler(
        IEmployerAccountRepository employerAccountRepository,
        IValidator<GetEmployerAccountByHashedIdQuery> validator)
    {
        _employerAccountRepository = employerAccountRepository;
        _validator = validator;
    }

    public async Task<GetEmployerAccountByHashedIdResponse> Handle(GetEmployerAccountByHashedIdQuery message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        if (result.IsUnauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        var employerAccount = await _employerAccountRepository.GetAccountById(message.AccountId);

        return new GetEmployerAccountByHashedIdResponse
        {
            Account = employerAccount
        };
    }
}