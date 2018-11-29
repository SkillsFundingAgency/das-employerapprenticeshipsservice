using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class HasRoleQueryHandler : IReadStoreRequestHandler<HasRoleQuery, bool>
    {
        private readonly IUsersRolesRepository _usersRolesRepository;

        public HasRoleQueryHandler(IUsersRolesRepository usersRolesRepository)
        {
            _usersRolesRepository = usersRolesRepository;
        }

        public async Task<bool> Handle(HasRoleQuery request, CancellationToken cancellationToken)
        {
            var userRoles = await _usersRolesRepository
                .CreateQuery()
                .Where(r => r.UserRef == request.UserRef
                          && r.AccountId == request.EmployerAccountId).ToListAsync(cancellationToken);

            return userRoles.Any(r => r.Roles.Any(role => request.UserRoles.Any(requestRole => requestRole == role)));
        }
    }
}
