using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount
{
    public class CreateAccountCommandHandler : AsyncRequestHandler<CreateAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountCommandHandler(IAccountRepository accountRepository)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
        }

        protected override async Task HandleCore(CreateAccountCommand message)
        {
            //TODO: Validate

            await _accountRepository.CreateAccount(message.UserId, message.CompanyNumber, message.CompanyName, message.EmployerRef);

            //TODO: Send message to queue
        }
    }
}