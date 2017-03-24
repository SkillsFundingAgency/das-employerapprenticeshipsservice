using System;
using System.Data;
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

            if (command.Commitment.EmployerLastUpdateInfo == null)
            {
                result.AddError(nameof(command.Commitment.EmployerLastUpdateInfo), $"{nameof(command.Commitment.EmployerLastUpdateInfo)} must have a value.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(command.Commitment.EmployerLastUpdateInfo.Name))
                result.AddError(nameof(command.Commitment.EmployerLastUpdateInfo.Name), $"{nameof(command.Commitment.EmployerLastUpdateInfo.Name)} must have a value.");

            if (string.IsNullOrWhiteSpace(command.Commitment.EmployerLastUpdateInfo.EmailAddress))
                result.AddError(nameof(command.Commitment.EmployerLastUpdateInfo.EmailAddress), $"{nameof(command.Commitment.EmployerLastUpdateInfo.EmailAddress)} must have a value.");

            if(command.UserId == null)
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} must have a value.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(CreateCommitmentCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
