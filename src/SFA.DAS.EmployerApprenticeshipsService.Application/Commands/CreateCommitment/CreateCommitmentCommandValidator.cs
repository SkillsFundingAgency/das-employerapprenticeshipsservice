using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandValidator : IValidator<CreateCommitmentCommand>
    {
        public ValidationResult Validate(CreateCommitmentCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new ValidationResult();

            if (command.Commitment == null)
            {
                result.AddError(nameof(command.Commitment), $"{nameof(command.Commitment)} must have a value.");
                return result;
            }

            if (command.Commitment == null)
                result.AddError(nameof(command.Commitment), $"{nameof(command.Commitment)} must have a value.");

            if (command.Commitment.EmployerAccountId <= 0)
                result.AddError(nameof(command.Commitment.EmployerAccountId), $"{nameof(command.Commitment.EmployerAccountId)} has an invalid value.");

            if (string.IsNullOrWhiteSpace(command.Commitment.LegalEntityId))
                result.AddError(nameof(command.Commitment.LegalEntityId), $"{nameof(command.Commitment.LegalEntityId)} has an invalid value.");

            if (string.IsNullOrWhiteSpace(command.Commitment.LegalEntityName))
                result.AddError(nameof(command.Commitment.LegalEntityName), $"{nameof(command.Commitment.LegalEntityName)} must have a value.");

            if (command.Commitment.ProviderId != 0 || !string.IsNullOrWhiteSpace(command.Commitment.ProviderName))
            {
                if (command.Commitment.ProviderId <= 0)
                    result.AddError(nameof(command.Commitment.ProviderId), $"{nameof(command.Commitment.ProviderId)} has an invalid value.");

                if (string.IsNullOrWhiteSpace(command.Commitment.ProviderName))
                    result.AddError(nameof(command.Commitment.ProviderName), $"{nameof(command.Commitment.ProviderName)} must have a value.");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(CreateCommitmentCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
