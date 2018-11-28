using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class UpdateUserRolesCommandHandler : IReadStoreRequestHandler<UpdateUserRolesCommand, Unit>
    {
        private readonly IUsersRolesRepository _usersRolesRepository;

        public UpdateUserRolesCommandHandler(IUsersRolesRepository usersRolesRepository)
        {
            _usersRolesRepository = usersRolesRepository;
        }
        public async Task<Unit> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRolesRepository.CreateQuery().SingleOrDefaultAsync(x => x.UserRef == request.UserRef && x.AccountId == request.AccountId, cancellationToken);

            if (user == null)
            {
                user = new UserRoles(request.UserRef, request.AccountId, request.Roles, request.MessageId,
                    request.Updated);
                await _usersRolesRepository.Add(user, null, cancellationToken);
            }
            else
            {
                user.UpdateRoles(request.Roles, request.Updated, request.MessageId);
                await _usersRolesRepository.Update(user, null, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
