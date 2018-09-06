﻿using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler : AsyncRequestHandler<UpsertRegisteredUserCommand>
    {
        private readonly Validation.IValidator<UpsertRegisteredUserCommand> _validator;
        private readonly IUserRepository _userRepository;

        public UpsertRegisteredUserCommandHandler(Validation.IValidator<UpsertRegisteredUserCommand> validator, IUserRepository userRepository)
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
                UserRef = message.UserRef,
                Email = message.EmailAddress,
                FirstName = message.FirstName,
                LastName = message.LastName
            });
        }
    }
}
