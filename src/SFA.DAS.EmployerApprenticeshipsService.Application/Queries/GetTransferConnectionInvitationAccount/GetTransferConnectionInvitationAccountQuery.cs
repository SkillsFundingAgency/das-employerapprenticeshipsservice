using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountQuery : IAsyncRequest<GetTransferConnectionInvitationAccountResponse>
    {
        [Required(ErrorMessage = "Account ID is required.")]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$", ErrorMessage = "Account ID must be 6 alphanumeric characters.")]
        public string ReceiverAccountHashedId { get; set; }

        [Required]
        public string SenderAccountHashedId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverAccountHashedId == SenderAccountHashedId)
            {
                yield return new ValidationResult("Account ID cannot be the current account's ID.", new[] { nameof(ReceiverAccountHashedId) });
            }
        }
    }
}