using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class StartTransferConnectionInvitationViewModel : MembershipMessage
    {
        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(EmployerAccounts.Constants.AccountHashedIdRegex, ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}