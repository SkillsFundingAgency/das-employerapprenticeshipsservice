using System.Threading;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;

public class GetUserAornLockQueryHandler : IRequestHandler<GetUserAornLockRequest, GetUserAornLockResponse>
{
    private readonly IUserAornPayeLockService _userAornPayeLockService;

    public GetUserAornLockQueryHandler(IUserAornPayeLockService userAornPayeLockService)
    {
        _userAornPayeLockService = userAornPayeLockService;
    }

    public async Task<GetUserAornLockResponse> Handle(GetUserAornLockRequest message, CancellationToken cancellationToken)
    {
        var status = await _userAornPayeLockService.UserAornPayeStatus(message.UserRef);
        return new GetUserAornLockResponse { UserAornStatus = status };
    }
}