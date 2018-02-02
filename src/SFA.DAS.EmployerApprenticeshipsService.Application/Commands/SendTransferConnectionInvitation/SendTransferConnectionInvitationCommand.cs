using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommand : IAsyncRequest<long>, IValidatableObject
    {
        [Required(ErrorMessage = "Account ID is required.")]
        [RegularExpression(Constants.HashedAccountIdRegex, ErrorMessage = "Account ID must be 6 alphanumeric characters.")]
        public string ReceiverAccountHashedId { get; set; }

        [Required]
        public string SenderAccountHashedId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverAccountHashedId?.ToLower() == SenderAccountHashedId?.ToLower())
            {
                yield return new ValidationResult("Account ID cannot be the current account's ID.", new [] { nameof(ReceiverAccountHashedId) });
            }
        }
    }
}