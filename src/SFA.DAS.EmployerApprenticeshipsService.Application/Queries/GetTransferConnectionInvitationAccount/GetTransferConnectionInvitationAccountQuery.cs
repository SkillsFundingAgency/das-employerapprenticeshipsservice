using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountQuery : IAsyncRequest<GetTransferConnectionInvitationAccountResponse>, IValidatableObject
    {
        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$", ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountHashedId { get; set; }

        [Required]
        public string SenderAccountHashedId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverAccountHashedId?.ToLower() == SenderAccountHashedId?.ToLower())
            {
                yield return new ValidationResult("You must enter a valid account ID", new[] { nameof(ReceiverAccountHashedId) });
            }
        }
    }
}