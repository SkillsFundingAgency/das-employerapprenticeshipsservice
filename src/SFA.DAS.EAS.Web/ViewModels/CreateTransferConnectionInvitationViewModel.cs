using SFA.DAS.EAS.Application.Commands.CreateTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CreateTransferConnectionInvitationViewModel : ViewModel<CreateTransferConnectionInvitationCommand>
    {
        public string SenderHashedAccountId { get; set; }
    }
}