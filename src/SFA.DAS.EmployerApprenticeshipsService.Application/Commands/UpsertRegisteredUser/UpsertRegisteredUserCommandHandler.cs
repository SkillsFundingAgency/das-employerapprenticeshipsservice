using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.UpsertRegisteredUser
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
            
            var user = await _userRepository.GetById(message.UserRef);
            
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
                await _userRepository.Update(new User
                {
                    Email = message.EmailAddress,
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    UserRef = message.UserRef
                });
            }
        }
    }
}
