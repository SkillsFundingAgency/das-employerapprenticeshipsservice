using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Validation;

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
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation($"Getting team members for account id {message.HashedAccountId}");

        var teamMembers = await _repository.GetAccountTeamMembers(message.HashedAccountId);

        return new GetTeamMembersResponse
        {
            TeamMembers = teamMembers
        };
    }
}