using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandHandler : AsyncRequestHandler<RemovePayeFromAccountCommand>
    {
        private readonly IValidator<RemovePayeFromAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;

        public RemovePayeFromAccountCommandHandler(IValidator<RemovePayeFromAccountCommand> validator, IAccountRepository accountRepository)
        {
            _validator = validator;
            _accountRepository = accountRepository;
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

            await _accountRepository.RemovePayeFromAccount(message.AccountId, message.PayeRef);

        }
    }
}
