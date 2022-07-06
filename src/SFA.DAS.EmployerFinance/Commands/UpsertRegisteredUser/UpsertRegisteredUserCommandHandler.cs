using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler : AsyncRequestHandler<UpsertRegisteredUserCommand>
    {
        private readonly IValidator<UpsertRegisteredUserCommand> _validator;
        private readonly ILog _logger;
        private readonly IUserRepository _userRepository;

        public UpsertRegisteredUserCommandHandler(
            IValidator<UpsertRegisteredUserCommand> validator,
            ILog logger,
            IUserRepository userRepository)
        {
            _validator = validator;
            _logger = logger;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(UpsertRegisteredUserCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid()) throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _userRepository.Upsert(new User
            {
                Ref = new Guid(message.UserRef),
                Email = message.EmailAddress,
                FirstName = message.FirstName,
                LastName = message.LastName,
                CorrelationId = message.CorrelationId
            });

            _logger.Info($"Upserted user with email={message.EmailAddress}, userRef={message.UserRef}, lastName={message.LastName}, firstName={message.FirstName}");
        }
    }
}
