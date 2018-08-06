using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Exceptions;

namespace SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd
{
    public class CreateNewPeriodEndCommandHandler : AsyncRequestHandler<CreateNewPeriodEndCommand>
    {
        private readonly IValidator<CreateNewPeriodEndCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;

        public CreateNewPeriodEndCommandHandler(IValidator<CreateNewPeriodEndCommand> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        protected override async Task HandleCore(CreateNewPeriodEndCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _dasLevyRepository.CreateNewPeriodEnd(message.NewPeriodEnd);
        }
    }
}