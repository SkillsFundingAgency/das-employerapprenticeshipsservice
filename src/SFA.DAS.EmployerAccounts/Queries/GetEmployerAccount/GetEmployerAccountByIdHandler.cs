using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByIdHandler : IRequestHandler<GetEmployerAccountByIdQuery, GetEmployerAccountByIdResponse>
{
    private readonly IEmployerAccountRepository _employerAccountRepository;
    private readonly IValidator<GetEmployerAccountByIdQuery> _validator;

    public GetEmployerAccountByIdHandler(
        IEmployerAccountRepository employerAccountRepository,
        IValidator<GetEmployerAccountByIdQuery> validator)
    {
        _employerAccountRepository = employerAccountRepository;
        _validator = validator;
    }

    public async Task<GetEmployerAccountByIdResponse> Handle(GetEmployerAccountByIdQuery message, CancellationToken cancellationToken)
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

        return new GetEmployerAccountByIdResponse
        {
            Account = employerAccount
        };
    }
}