using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity
{
    public class AddPayeToNewLegalEnttiyCommandHandler : AsyncRequestHandler<AddPayeToNewLegalEntityCommand>
    {
        private readonly IValidator<AddPayeToNewLegalEntityCommand> _validator;
        private readonly IAccountRepository _accountRepository;

        public AddPayeToNewLegalEnttiyCommandHandler(IValidator<AddPayeToNewLegalEntityCommand> validator, IAccountRepository accountRepository)
        {
            _validator = validator;
            _accountRepository = accountRepository;
        }

        protected override async Task HandleCore(AddPayeToNewLegalEntityCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            await _accountRepository.AddPayeToAccountForNewLegalEntity(
                    new Paye
                    {
                        AccessToken = message.AccessToken,
                        RefreshToken = message.RefreshToken,
                        AccountId = message.AccountId,
                        EmpRef = message.Empref
                    }, 
                    new LegalEntity
                    {
                        Name = message.LegalEntityName,
                        Code = message.LegalEntityCode,
                        DateOfIncorporation = message.LegalEntityDate,
                        RegisteredAddress = message.LegalEntityAddress
                    }
                );

        }
    }
}