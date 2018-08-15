using MediatR;
using SFA.DAS.EmployerAccounts.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<SendTransferConnectionInvitationResponse>
    {
        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(Constants.AccountHashedIdRegex, ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}