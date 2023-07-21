using System.Threading;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;

public class GetAccountTeamMembersHandler : IRequestHandler<GetAccountTeamMembersQuery, GetAccountTeamMembersResponse>
{
    private readonly IValidator<GetAccountTeamMembersQuery> _validator;
    private readonly IEmployerAccountTeamRepository _repository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public GetAccountTeamMembersHandler(
        IValidator<GetAccountTeamMembersQuery> validator, 
        IEmployerAccountTeamRepository repository,
        IMediator mediator, 
        IMembershipRepository membershipRepository, IUserContext userContext)
    {
        _validator = validator;
        _repository = repository;
        _mediator = mediator;
        _membershipRepository = membershipRepository;
        _userContext = userContext;
    }

    public async Task<GetAccountTeamMembersResponse> Handle(GetAccountTeamMembersQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException("User not authorised");
            }

            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accounts = await _repository.GetAccountTeamMembersForUserId(message.HashedAccountId, message.ExternalUserId);

        if (_userContext.IsSupportConsoleUser())
        {
            await AuditAccess(message);
        }

        return new GetAccountTeamMembersResponse { TeamMembers = accounts };
    }

    private async Task AuditAccess(GetAccountTeamMembersQuery message)
    {
        var caller = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "VIEW",
                Description = $"Account {caller.AccountId} team members viewed",
                AffectedEntity = new AuditEntity { Type = "TeamMembers", Id = caller.AccountId.ToString() }
            }
        });
    }
}