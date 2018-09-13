using SFA.DAS.EmployerFinance.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class ApprovedTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("GoToApprenticesPage|GoToHomepage", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}