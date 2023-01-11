namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;

public class UpdateTermAndConditionsAcceptedOnCommandHandler : AsyncRequestHandler<UpdateTermAndConditionsAcceptedOnCommand>
{
    private readonly IUserRepository _userRepository;
    public UpdateTermAndConditionsAcceptedOnCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    protected override async Task HandleCore(UpdateTermAndConditionsAcceptedOnCommand message)
    {
        await _userRepository.UpdateTermAndConditionsAcceptedOn(message.UserRef);
    }
}