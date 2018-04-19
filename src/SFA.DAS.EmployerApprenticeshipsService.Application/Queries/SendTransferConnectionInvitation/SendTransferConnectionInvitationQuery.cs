using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<SendTransferConnectionInvitationResponse>
    {
        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(Constants.AccountHashedIdRegex, ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}