using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandHandler : AsyncRequestHandler<RenameEmployerAccountCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IValidator<RenameEmployerAccountCommand> _validator;
        private readonly IHashingService _hashingService;

        public RenameEmployerAccountCommandHandler(IEmployerAccountRepository accountRepository, IValidator<RenameEmployerAccountCommand> validator, IHashingService hashingService)
        {
            _accountRepository = accountRepository;
            _validator = validator;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(RenameEmployerAccountCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await _accountRepository.RenameAccount(accountId, message.NewName);
        }
    }
}
