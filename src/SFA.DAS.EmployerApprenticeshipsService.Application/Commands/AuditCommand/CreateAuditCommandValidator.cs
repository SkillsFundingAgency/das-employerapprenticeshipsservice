using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Commands.AuditCommand
{
    public class CreateAuditCommandValidator : IValidator<CreateAuditCommand>
    {
        public ValidationResult Validate(CreateAuditCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.EasAuditMessage == null)
            {
                validationResult.AddError(nameof(item.EasAuditMessage));
                return validationResult;
            }

            if (string.IsNullOrEmpty(item.EasAuditMessage.Description))
            {
                validationResult.AddError(nameof(item.EasAuditMessage.Description));
            }

            if (item.EasAuditMessage.ChangedProperties==null || !item.EasAuditMessage.ChangedProperties.Any())
            {
                validationResult.AddError(nameof(item.EasAuditMessage.ChangedProperties));
            }
            
            if (string.IsNullOrEmpty(item.EasAuditMessage.AffectedEntity?.Id) || string.IsNullOrEmpty(item.EasAuditMessage.AffectedEntity?.Type))
            {
                validationResult.AddError(nameof(item.EasAuditMessage.AffectedEntity));
            }
            
            return validationResult;

        }

        public Task<ValidationResult> ValidateAsync(CreateAuditCommand item)
        {
            throw new NotImplementedException();
        }
    }
}