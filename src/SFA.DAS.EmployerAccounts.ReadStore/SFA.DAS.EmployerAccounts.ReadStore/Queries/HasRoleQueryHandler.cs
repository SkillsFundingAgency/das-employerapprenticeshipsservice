using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Queries
{
    internal class HasRoleQueryHandler : IApiRequestHandler<HasRoleQuery, HasRoleQueryResult>
    {
        private readonly IUsersRolesRepository _usersRolesRepository;

        public HasRoleQueryHandler(IUsersRolesRepository usersRolesRepository)
        {
            _usersRolesRepository = usersRolesRepository;
        }

        public async Task<HasRoleQueryResult> Handle(HasRoleQuery request, CancellationToken cancellationToken)
        {
            var userRoles = await _usersRolesRepository
                .CreateQuery()
                .Where(r => r.UserRef == request.UserRef
                          && r.AccountId == request.EmployerAccountId).ToListAsync(cancellationToken);

            var hasRole = userRoles.Any(r => r.Roles.Any(role => request.UserRoles.Any(requestRole => (short)requestRole == role)));

            return new HasRoleQueryResult{ HasRole = hasRole };
        }
    }
}
