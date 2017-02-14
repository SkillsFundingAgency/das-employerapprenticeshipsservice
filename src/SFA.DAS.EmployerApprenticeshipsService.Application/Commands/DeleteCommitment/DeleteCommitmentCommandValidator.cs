using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.DeleteCommitment
{
    public class DeleteCommitmentCommandValidator : IValidator<DeleteCommitmentCommand>
    {

        public ValidationResult Validate(DeleteCommitmentCommand item)
        {
            return ValidateCommand(item);
        }

        public Task<ValidationResult> ValidateAsync(DeleteCommitmentCommand item)
        {
            return Task.Run(() => ValidateCommand(item));
        }

        private static ValidationResult ValidateCommand(DeleteCommitmentCommand item)
        {
            var result = new ValidationResult();

            if (item.CommitmentId < 1)
            {
                result.AddError("ApprenticeshipId", "No ApprenticeshipId supplied");
            }

            if (item.AccountId < 1)
            {
                result.AddError("AccountId", "No AccountId supplied");
            }

            return result;
        }
    }
}