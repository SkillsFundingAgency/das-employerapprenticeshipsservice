using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class ApprovedTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("GoToApprenticesPage|GoToHomepage", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}