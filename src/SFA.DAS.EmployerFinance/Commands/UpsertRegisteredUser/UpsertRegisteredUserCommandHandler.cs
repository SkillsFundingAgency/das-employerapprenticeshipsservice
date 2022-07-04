using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.Validation;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler : RequestHandler<UpsertRegisteredUserCommand>
    {
        private readonly IValidator<UpsertRegisteredUserCommand> _validator;
        private readonly IUserAccountRepository _userRepository;

        public UpsertRegisteredUserCommandHandler(
            IValidator<UpsertRegisteredUserCommand> validator,
            IUserAccountRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        protected override void HandleCore(UpsertRegisteredUserCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid()) throw new InvalidRequestException(validationResult.ValidationDictionary);

            _userRepository.Upsert(new User
            {
                Ref = new Guid(message.UserRef),
                Email = message.EmailAddress,
                FirstName = message.FirstName,
                LastName = message.LastName,
                CorrelationId = message.CorrelationId
            });
        }
    }
}
