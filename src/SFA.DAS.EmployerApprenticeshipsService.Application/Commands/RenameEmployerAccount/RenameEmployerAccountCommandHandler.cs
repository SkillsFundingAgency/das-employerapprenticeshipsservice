using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandHandler : AsyncRequestHandler<RenameEmployerAccountCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IValidator<RenameEmployerAccountCommand> _validator;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;

        public RenameEmployerAccountCommandHandler(IEmployerAccountRepository accountRepository, IValidator<RenameEmployerAccountCommand> validator, IHashingService hashingService, IMediator mediator)
        {
            _accountRepository = accountRepository;
            _validator = validator;
            _hashingService = hashingService;
            _mediator = mediator;
        }

        protected override async Task HandleCore(RenameEmployerAccountCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await _accountRepository.RenameAccount(accountId, message.NewName);
            await _mediator.PublishAsync(new CreateAccountEventCommand
            {
                HashedAccountId = message.HashedAccountId,
                Event = "AccountRenamed"
            });
        }
    }
}
