namespace SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;

public class UpdateUserAornLockQueryHandler : AsyncRequestHandler<UpdateUserAornLockRequest>
{
    private readonly IUserAornPayeLockService _userAornPayeLockService;

    public UpdateUserAornLockQueryHandler(IUserAornPayeLockService userAornPayeLockService)
    {
        _userAornPayeLockService = userAornPayeLockService;
    }

    protected override async Task HandleCore(UpdateUserAornLockRequest message)
    {
        await _userAornPayeLockService.UpdateUserAornPayeAttempt(message.UserRef, message.Success);
    }
}