using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountQuery : IAsyncRequest<GetTransferConnectionInvitationAccountResponse>, IValidatableObject
    {
        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(Constants.HashedAccountIdRegex, ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountPublicHashedId { get; set; }

        [Required]
        public string SenderAccountHashedId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverAccountPublicHashedId?.ToLower() == SenderAccountHashedId?.ToLower())
            {
                yield return new ValidationResult("You must enter a valid account ID", new[] { nameof(ReceiverAccountPublicHashedId) });
            }
        }
    }
}