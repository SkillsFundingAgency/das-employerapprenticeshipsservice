using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest<long>
    {
        [Required]
        [RegularExpression(Constants.AccountHashedIdRegex)]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}