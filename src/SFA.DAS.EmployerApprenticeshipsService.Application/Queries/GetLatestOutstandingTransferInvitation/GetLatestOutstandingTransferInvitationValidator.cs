using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation
{
    public class GetLatestOutstandingTransferInvitation : IValidator<GetLatestOutstandingTransferInvitationQuery>
    {
        public ValidationResult Validate(GetLatestOutstandingTransferInvitationQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.ReceiverAccountHashedId))
            {
                validationResult.AddError(nameof(item.ReceiverAccountHashedId), "Receiver account not supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetLatestOutstandingTransferInvitationQuery item)
        {
            throw new NotImplementedException();
        }
    }
}