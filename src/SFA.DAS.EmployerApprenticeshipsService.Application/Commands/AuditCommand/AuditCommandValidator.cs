using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.AuditCommand
{
    public class AuditCommandValidator : IValidator<AuditCommand>
    {
        public ValidationResult Validate(AuditCommand item)
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

            if (item.EasAuditMessage.RelatedEntities == null || !item.EasAuditMessage.RelatedEntities.Any())
            {
                validationResult.AddError(nameof(item.EasAuditMessage.RelatedEntities));
            }

            return validationResult;

        }

        public Task<ValidationResult> ValidateAsync(AuditCommand item)
        {
            throw new NotImplementedException();
        }
    }
}