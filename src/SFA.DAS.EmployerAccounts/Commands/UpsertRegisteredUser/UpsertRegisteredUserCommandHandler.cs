using System;
using MediatR;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler : AsyncRequestHandler<UpsertRegisteredUserCommand>
    {
        private readonly IValidator<UpsertRegisteredUserCommand> _validator;
        private readonly IUserAccountRepository _userRepository;
        private readonly IEventPublisher _eventPublisher;

        public UpsertRegisteredUserCommandHandler(IValidator<UpsertRegisteredUserCommand> validator,
            IUserAccountRepository userRepository, IEventPublisher eventPublisher)
        {
            _validator = validator;
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
        }

        protected override async Task HandleCore(UpsertRegisteredUserCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _userRepository.Upsert(new User
            {
                UserRef = message.UserRef,
                Email = message.EmailAddress,
                FirstName = message.FirstName,
                LastName = message.LastName
            });

            await _eventPublisher.Publish(new UpsertedUserEvent {Created = DateTime.UtcNow, UserRef = message.UserRef});
        }
    }
}
