using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;

public class GetPagedAccountsQueryHandler : IAsyncRequestHandler<GetPagedEmployerAccountsQuery, GetPagedEmployerAccountsResponse>
{
    private readonly IEmployerAccountRepository _employerAccountRepository;
    private readonly IValidator<GetPagedEmployerAccountsQuery> _validator;

    public GetPagedAccountsQueryHandler(
        IValidator<GetPagedEmployerAccountsQuery> validator, 
        IEmployerAccountRepository employerAccountRepository)
    {
        _employerAccountRepository = employerAccountRepository;
        _validator = validator;
    }

    public async Task<GetPagedEmployerAccountsResponse> Handle(GetPagedEmployerAccountsQuery message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accounts = await _employerAccountRepository.GetAccounts(message.ToDate, message.PageNumber, message.PageSize);
        return new GetPagedEmployerAccountsResponse() { AccountsCount = accounts.AccountsCount, Accounts = accounts.AccountList };
    }
}