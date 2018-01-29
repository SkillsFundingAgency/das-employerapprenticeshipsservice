using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class StartTransferConnectionInvitationViewModel : ViewModel<GetTransferConnectionInvitationAccountQuery>
    {
        public string SenderAccountHashedId { get; set; }
    }
}