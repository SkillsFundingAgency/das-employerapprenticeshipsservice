using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUserAccountRepository _repository;
    private readonly IValidator<GetUserQuery> _validator;

    public GetUserQueryHandler(IUserAccountRepository repository, IValidator<GetUserQuery> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async  Task<GetUserResponse> Handle(GetUserQuery message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var user = await _repository.Get(message.UserId);

        return new GetUserResponse {User = user};
    }
}