using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;

internal class IsUserInAnyRoleQueryHandler : IRequestHandler<IsUserInAnyRoleQuery, bool>
{
    private readonly IAccountUsersRepository _accountUsersRepository;

    public IsUserInAnyRoleQueryHandler(IAccountUsersRepository accountUsersRepository)
    {
        _accountUsersRepository = accountUsersRepository;
    }

    public Task<bool> Handle(IsUserInAnyRoleQuery request, CancellationToken cancellationToken)
    {
        return _accountUsersRepository
            .CreateQuery()
            .AnyAsync(r => 
                r.UserRef == request.UserRef && 
                r.AccountId == request.AccountId && 
                r.Removed == null, cancellationToken);
    }
}