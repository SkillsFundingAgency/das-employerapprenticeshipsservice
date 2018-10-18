using System.ComponentModel.DataAnnotations;
using SFA.DAS.EmployerAccounts.Queries.SendTransferConnectionInvitation;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class StartTransferConnectionInvitationViewModel
    {
        [Required]
        public SendTransferConnectionInvitationQuery SendTransferConnectionInvitationQuery { get; set; }
    }
}