using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandHandler : AsyncRequestHandler<RemovePayeFromAccountCommand>
    {
        private readonly IValidator<RemovePayeFromAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IHashingService _hashingService;

        public RemovePayeFromAccountCommandHandler(IValidator<RemovePayeFromAccountCommand> validator, IAccountRepository accountRepository, IHashingService hashingService)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(RemovePayeFromAccountCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }
            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            await _accountRepository.RemovePayeFromAccount(_hashingService.DecodeValue(message.HashedId), message.PayeRef);

        }
    }
}
