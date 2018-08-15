using MediatR;
using SFA.DAS.EmployerAccounts.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class SendTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest<long>
    {
        [Required]
        [RegularExpression(Constants.AccountHashedIdRegex)]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}