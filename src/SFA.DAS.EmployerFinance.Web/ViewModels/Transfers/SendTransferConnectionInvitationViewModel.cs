using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class SendTransferConnectionInvitationViewModel : IAuthorizationContextModel
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public Guid UserRef { get; set; }

        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Confirm|ReEnterAccountId", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public AccountDto ReceiverAccount { get; set; }
        public AccountDto SenderAccount { get; set; }

        [Required]
        [RegularExpression(EmployerFinance.Constants.AccountHashedIdRegex)]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}