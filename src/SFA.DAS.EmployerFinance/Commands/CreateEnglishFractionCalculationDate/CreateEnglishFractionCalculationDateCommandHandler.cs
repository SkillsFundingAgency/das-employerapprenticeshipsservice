using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Exceptions;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.CreateEnglishFractionCalculationDate
{
    public class CreateEnglishFractionCalculationDateCommandHandler : AsyncRequestHandler<CreateEnglishFractionCalculationDateCommand>
    {
        private readonly IValidator<CreateEnglishFractionCalculationDateCommand> _validator;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly ILog _logger;

        public CreateEnglishFractionCalculationDateCommandHandler(IValidator<CreateEnglishFractionCalculationDateCommand> validator, IEnglishFractionRepository englishFractionRepository, ILog logger)
        {
            _validator = validator;
            _englishFractionRepository = englishFractionRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(CreateEnglishFractionCalculationDateCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _englishFractionRepository.SetLastUpdateDate(message.DateCalculated);

            _logger.Info($"English Fraction CalculationDate updated to {message.DateCalculated.ToString("dd MMM yyyy")}");
        }
    }
}
