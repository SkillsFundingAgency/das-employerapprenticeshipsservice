using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateApprenticeship
{
    public sealed class CreateApprenticeshipCommandValidator : IValidator<CreateApprenticeshipCommand>
    {
        public ValidationResult Validate(CreateApprenticeshipCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId <= 0)
                validationResult.AddError(nameof(item.AccountId), $"{nameof(item.AccountId)} has an invalid value");

            if (item.Apprenticeship.CommitmentId <= 0)
                validationResult.AddError(nameof(item.Apprenticeship.CommitmentId), $"{nameof(item.Apprenticeship.CommitmentId)} has an invalid value");

            if (item.Apprenticeship.ULN != null)
            {
                if (item.Apprenticeship.ULN.Length != 10 || !item.Apprenticeship.ULN.All(x => char.IsDigit(x)))
                    validationResult.AddError(nameof(item.Apprenticeship.ULN), $"{nameof(item.Apprenticeship.ULN)} must be numeric and 10 digits in length.");
            }

            if (item.Apprenticeship.Cost != null)
            {
                if (item.Apprenticeship.Cost <= 0 || HasGreaterThan2DecimalPlaces(item))
                    validationResult.AddError(nameof(item.Apprenticeship.Cost), $"{nameof(item.Apprenticeship.Cost)} must be non-zero and 2 decimal places only.");
            }

            return validationResult;
        }

        private static bool HasGreaterThan2DecimalPlaces(CreateApprenticeshipCommand item)
        {
            return decimal.GetBits(item.Apprenticeship.Cost.Value)[3] >> 16 > 2;
        }

        public Task<ValidationResult> ValidateAsync(CreateApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}