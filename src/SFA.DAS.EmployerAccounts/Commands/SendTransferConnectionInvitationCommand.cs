using MediatR;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Commands
{
    public class SendTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest<long>
    {
        [Required]
        [RegularExpression(Constants.AccountHashedIdRegex)]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}