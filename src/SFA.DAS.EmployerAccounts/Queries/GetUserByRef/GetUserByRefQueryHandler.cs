using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

public class GetUserByRefQueryHandler : IRequestHandler<GetUserByRefQuery, GetUserByRefResponse>
{
    private readonly IUserRepository _repository;
    private readonly IValidator<GetUserByRefQuery> _validator;
    private readonly ILogger<GetUserByRefQueryHandler> _logger;

    public GetUserByRefQueryHandler(IUserRepository repository, IValidator<GetUserByRefQuery> validator, ILogger<GetUserByRefQueryHandler> logger)
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

        _logger.LogDebug("Getting user with ref {UserRef}", message.UserRef);

        var user = await _repository.GetUserByRef(message.UserRef);

        if (user == null)
        {
            validationResult.AddError(nameof(message.UserRef), "User does not exist");
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        return new GetUserByRefResponse{ User = user};
    }
}