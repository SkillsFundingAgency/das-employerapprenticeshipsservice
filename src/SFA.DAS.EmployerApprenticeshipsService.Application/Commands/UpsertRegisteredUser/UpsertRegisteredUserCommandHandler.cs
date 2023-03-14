using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using System;

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

            await _userRepository.Upsert(new User
            {
                Ref = Guid.Parse(message.UserRef),
                Email = message.EmailAddress,
                FirstName = message.FirstName,
                LastName = message.LastName
            });
        }
    }
}
