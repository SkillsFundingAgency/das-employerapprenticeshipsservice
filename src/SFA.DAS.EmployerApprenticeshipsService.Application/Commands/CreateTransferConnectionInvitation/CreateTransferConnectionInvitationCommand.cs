using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateTransferConnectionInvitation
{
    public class CreateTransferConnectionInvitationCommand : IAsyncRequest<long>, IValidatableObject
    {
        [Required]
        public string SenderHashedAccountId { get; set; }
        
        [Required(ErrorMessage = "Account ID is required.")]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$", ErrorMessage = "Account ID must be 6 alphanumeric characters.")]
        public string ReceiverHashedAccountId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReceiverHashedAccountId == SenderHashedAccountId)
            {
                yield return new ValidationResult("Account ID cannot be the current account's ID.", new[] { nameof(ReceiverHashedAccountId) });
            }
        }
    }
}