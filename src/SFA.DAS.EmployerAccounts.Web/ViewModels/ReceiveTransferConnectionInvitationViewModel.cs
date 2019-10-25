using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class ReceiveTransferConnectionInvitationViewModel : IAuthorizationContextModel
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public Guid UserRef { get; set; }

        [Required]
        public int? TransferConnectionInvitationId { get; set; }

        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Approve|Reject", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}