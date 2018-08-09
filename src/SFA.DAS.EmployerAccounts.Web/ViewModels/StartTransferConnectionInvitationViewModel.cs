using SFA.DAS.EmployerAccounts.Queries.SendTransferConnectionInvitation;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class StartTransferConnectionInvitationViewModel
    {
        [Required]
        public SendTransferConnectionInvitationQuery SendTransferConnectionInvitationQuery { get; set; }
    }
}