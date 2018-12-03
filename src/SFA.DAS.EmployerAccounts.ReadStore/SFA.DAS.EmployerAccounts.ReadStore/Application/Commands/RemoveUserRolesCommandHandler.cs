using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class RemoveUserRolesCommandHandler : IReadStoreRequestHandler<RemoveUserRolesCommand, Unit>
    {
        private readonly IUsersRolesRepository _usersRolesRepository;

        public RemoveUserRolesCommandHandler(IUsersRolesRepository usersRolesRepository)
        {
            _usersRolesRepository = usersRolesRepository;
        }
        public async Task<Unit> Handle(RemoveUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRolesRepository.CreateQuery().SingleAsync(x => x.UserId == request.UserId && x.AccountId == request.AccountId, cancellationToken);

            user.Remove(request.Removed, request.MessageId);
            await _usersRolesRepository.Update(user, null, cancellationToken);

            return Unit.Value;
        }
    }
}
