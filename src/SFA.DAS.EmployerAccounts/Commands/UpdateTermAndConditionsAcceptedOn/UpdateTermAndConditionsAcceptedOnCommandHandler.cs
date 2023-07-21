using System.Threading;

namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;

public class UpdateTermAndConditionsAcceptedOnCommandHandler : IRequestHandler<UpdateTermAndConditionsAcceptedOnCommand>
{
    private readonly IUserRepository _userRepository;
    public UpdateTermAndConditionsAcceptedOnCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(UpdateTermAndConditionsAcceptedOnCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.UpdateTermAndConditionsAcceptedOn(request.UserRef);

        return Unit.Value;
    }
}