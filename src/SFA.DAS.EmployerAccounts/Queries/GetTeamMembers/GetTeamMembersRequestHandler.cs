using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequestHandler : IRequestHandler<GetTeamMembersRequest, GetTeamMembersResponse>
{
    private readonly IEmployerAccountTeamRepository _repository;
    private readonly IValidator<GetTeamMembersRequest> _validator;
    private readonly ILogger<GetTeamMembersRequestHandler> _logger;

    public GetTeamMembersRequestHandler(
        IEmployerAccountTeamRepository repository, 
        IValidator<GetTeamMembersRequest> validator,
        ILogger<GetTeamMembersRequestHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<GetTeamMembersResponse> Handle(GetTeamMembersRequest message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation("Getting team members for account id {AccountId}", message.AccountId);

        var teamMembers = await _repository.GetAccountTeamMembers(message.AccountId);

        return new GetTeamMembersResponse
        {
            TeamMembers = teamMembers
        };
    }
}