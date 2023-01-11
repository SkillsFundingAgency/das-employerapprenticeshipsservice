using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;

public class GetEmployerAccountDetailByHashedIdHandler : IAsyncRequestHandler<GetEmployerAccountDetailByHashedIdQuery, GetEmployerAccountDetailByHashedIdResponse>
{
    private readonly IValidator<GetEmployerAccountDetailByHashedIdQuery> _validator;
    private readonly IEmployerAccountRepository _employerAccountRepository;

    public GetEmployerAccountDetailByHashedIdHandler(IValidator<GetEmployerAccountDetailByHashedIdQuery> validator, IEmployerAccountRepository employerAccountRepository)
    {
        _validator = validator;
        _employerAccountRepository = employerAccountRepository;
    }

    public async Task<GetEmployerAccountDetailByHashedIdResponse> Handle(GetEmployerAccountDetailByHashedIdQuery message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var account = await _employerAccountRepository.GetAccountDetailByHashedId(message.HashedAccountId);

        return new GetEmployerAccountDetailByHashedIdResponse { Account = account };
    }
}