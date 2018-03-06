using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationAccountResponse>//, IValidatableObject
    {
        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(Constants.AccountHashedIdRegex, ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountPublicHashedId { get; set; }

        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverAccountPublicHashedId?.ToLower() == AccountPublicHashedId?.ToLower())
            {
                yield return new ValidationResult("You must enter a valid account ID", new[] { nameof(ReceiverAccountPublicHashedId) });
            }
        }*/
    }
}