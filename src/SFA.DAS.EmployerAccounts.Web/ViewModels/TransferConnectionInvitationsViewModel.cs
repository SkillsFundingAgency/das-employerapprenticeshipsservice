using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferConnectionInvitationsViewModel : IAuthorizationContextModel
    {
        public long AccountId { get; set; }

        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }

        public IEnumerable<TransferConnectionInvitationDto> TransferSenderConnectionInvitations =>
            TransferConnectionInvitations.Where(p => p.SenderAccount.Id == AccountId);

        public IEnumerable<TransferConnectionInvitationDto> TransferReceiverConnectionInvitations =>
            TransferConnectionInvitations.Where(p => p.ReceiverAccount.Id == AccountId);
    }
}