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
        private readonly IAccountUsersRepository _accountUsersRepository;

        public HasRoleQueryHandler(IAccountUsersRepository accountUsersRepository)
        {
            _accountUsersRepository = accountUsersRepository;
        }

        public async Task<bool> Handle(HasRoleQuery request, CancellationToken cancellationToken)
        {
            var user = await _accountUsersRepository
                .CreateQuery()
                .SingleOrDefaultAsync(r => r.UserRef == request.UserRef && r.AccountId == request.EmployerAccountId && r.Removed == null, cancellationToken);

            if (user == null)
                return false;

            return user.Roles.Any(role => request.UserRoles.Any(requestRole => requestRole == role));
        }
    }
}