using System.Threading;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

public class GetUserByRefQueryHandler : IRequestHandler<GetUserByRefQuery, GetUserByRefResponse>
{
    private readonly IUserRepository _repository;
    private readonly IValidator<GetUserByRefQuery> _validator;
    private readonly ILog _logger;

    public GetUserByRefQueryHandler(IUserRepository repository, IValidator<GetUserByRefQuery> validator, ILog logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<GetUserByRefResponse> Handle(GetUserByRefQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.Debug($"Getting user with ref {message.UserRef}");

        var user = await _repository.GetUserByRef(message.UserRef);

        if (user == null)
        {
            validationResult.AddError(nameof(message.UserRef), "User does not exist");
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        return new GetUserByRefResponse{ User = user};
    }
}