using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommand : AuthorizedMessage, IAsyncRequest<long>, IValidatableObject
    {
        [Required]
        [RegularExpression(Constants.AccountHashedIdRegex)]
        public string ReceiverAccountPublicHashedId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverAccountPublicHashedId?.ToLower() == AccountPublicHashedId?.ToLower())
            {
                yield return new ValidationResult("You must enter a valid account ID", new [] { nameof(ReceiverAccountPublicHashedId) });
            }
        }
    }
}