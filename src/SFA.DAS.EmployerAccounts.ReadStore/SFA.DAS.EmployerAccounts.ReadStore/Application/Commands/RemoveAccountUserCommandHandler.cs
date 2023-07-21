using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

internal class RemoveAccountUserCommandHandler : IRequestHandler<RemoveAccountUserCommand>
{
    private readonly IAccountUsersRepository _accountUsersRepository;

    public RemoveAccountUserCommandHandler(IAccountUsersRepository accountUsersRepository)
    {
        _accountUsersRepository = accountUsersRepository;
    }

    public async Task<Unit> Handle(RemoveAccountUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _accountUsersRepository.CreateQuery().SingleAsync(x => x.UserRef == request.UserRef && x.AccountId == request.AccountId, cancellationToken);

        user.Remove(request.Removed, request.MessageId);
        await _accountUsersRepository.Update(user, null, cancellationToken);

        return Unit.Value;
    }
}