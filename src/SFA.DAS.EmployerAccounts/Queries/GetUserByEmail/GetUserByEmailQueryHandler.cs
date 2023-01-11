using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IAsyncRequestHandler<GetUserByEmailQuery, GetUserByEmailResponse>
{
    private readonly IUserAccountRepository _repository;
    private readonly IValidator<GetUserByEmailQuery> _validator;

    public GetUserByEmailQueryHandler(IUserAccountRepository repository, IValidator<GetUserByEmailQuery> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async  Task<GetUserByEmailResponse> Handle(GetUserByEmailQuery message)
    {
        var result = _validator.Validate(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var user = await _repository.Get(message.Email);

        return new GetUserByEmailResponse {User = user};
    }
}