using SFA.DAS.EmployerAccounts.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class ApprovedTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("GoToApprenticesPage|GoToHomepage", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}