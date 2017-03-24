using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate
{
    public class CreateEnglishFractionCalculationDateCommandHandler : AsyncRequestHandler<CreateEnglishFractionCalculationDateCommand>
    {
        private readonly IValidator<CreateEnglishFractionCalculationDateCommand> _validator;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly ILogger _logger;

        public CreateEnglishFractionCalculationDateCommandHandler(IValidator<CreateEnglishFractionCalculationDateCommand> validator, IEnglishFractionRepository englishFractionRepository, ILogger logger)
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
