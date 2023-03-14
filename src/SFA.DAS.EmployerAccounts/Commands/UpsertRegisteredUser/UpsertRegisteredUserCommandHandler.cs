using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;

public class UpsertRegisteredUserCommandHandler : IRequestHandler<UpsertRegisteredUserCommand>
{
    private readonly IValidator<UpsertRegisteredUserCommand> _validator;
    private readonly IUserAccountRepository _userRepository;
    private readonly IEventPublisher _eventPublisher;

    public UpsertRegisteredUserCommandHandler(
        IValidator<UpsertRegisteredUserCommand> validator,
        IUserAccountRepository userRepository,
        IEventPublisher eventPublisher)
    {
        _validator = validator;
        _userRepository = userRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(UpsertRegisteredUserCommand message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid()) throw new InvalidRequestException(validationResult.ValidationDictionary);

        await _userRepository.Upsert(new User
        {
            UserRef = message.UserRef,
            Email = message.EmailAddress,
            FirstName = message.FirstName,
            LastName = message.LastName,
            CorrelationId = message.CorrelationId
        });

        await _eventPublisher.Publish(new UpsertedUserEvent { Created = DateTime.UtcNow, UserRef = message.UserRef, CorrelationId = message.CorrelationId });

        return Unit.Value;
    }
}