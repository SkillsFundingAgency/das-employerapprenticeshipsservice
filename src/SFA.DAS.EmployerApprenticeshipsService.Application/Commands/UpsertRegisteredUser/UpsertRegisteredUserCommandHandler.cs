using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler : AsyncRequestHandler<UpsertRegisteredUserCommand>
    {
        private readonly IValidator<UpsertRegisteredUserCommand> _validator;
        private readonly IUserRepository _userRepository;

        public UpsertRegisteredUserCommandHandler(IValidator<UpsertRegisteredUserCommand> validator, IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(UpsertRegisteredUserCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var user = await _userRepository.GetByUserRef(message.UserRef);

            if (user == null)
            {
                await _userRepository.Create(new User
                {
                    Email = message.EmailAddress,
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    UserRef = message.UserRef
                });
            }
            else
            {
                user.Email = message.EmailAddress;
                user.FirstName = message.FirstName;
                user.LastName = message.LastName;

                await _userRepository.Update(user);
            }
        }
    }
}
