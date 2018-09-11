using SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class StartTransferConnectionInvitationViewModel
    {
        [Required]
        public SendTransferConnectionInvitationQuery SendTransferConnectionInvitationQuery { get; set; }
    }
}