using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUser;

public class GetUserQueryHandler : IAsyncRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUserAccountRepository _repository;
    private readonly IValidator<GetUserQuery> _validator;

    public GetUserQueryHandler(IUserAccountRepository repository, IValidator<GetUserQuery> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async  Task<GetUserResponse> Handle(GetUserQuery message)
    {
        var result = _validator.Validate(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var user = await _repository.Get(message.UserId);

        return new GetUserResponse {User = user};
    }
}