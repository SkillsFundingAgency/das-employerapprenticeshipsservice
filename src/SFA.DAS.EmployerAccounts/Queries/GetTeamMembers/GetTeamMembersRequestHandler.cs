using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequestHandler : IAsyncRequestHandler<GetTeamMembersRequest, GetTeamMembersResponse>
{
    private readonly IEmployerAccountTeamRepository _repository;
    private readonly IValidator<GetTeamMembersRequest> _validator;
    private readonly ILog _logger;

    public GetTeamMembersRequestHandler(
        IEmployerAccountTeamRepository repository, 
        IValidator<GetTeamMembersRequest> validator,
        ILog logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<GetTeamMembersResponse> Handle(GetTeamMembersRequest message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.Info($"Getting team members for account id {message.HashedAccountId}");

        var teamMembers = await _repository.GetAccountTeamMembers(message.HashedAccountId);

        return new GetTeamMembersResponse
        {
            TeamMembers = teamMembers
        };
    }
}