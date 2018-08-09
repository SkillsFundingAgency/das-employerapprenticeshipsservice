using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class DeletedTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("GoToTransfersPage|GoToHomepage", ErrorMessage = "Option required")]
        public string Choice { get; set; }
    }
}