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
                .SingleOrDefaultAsync(r => r.UserRef == request.UserRef && r.AccountId == request.AccountId && r.Removed == null, cancellationToken);

            if (user == null)
                return false;

            return user.HasRole(request.UserRoles);
        }
    }
}