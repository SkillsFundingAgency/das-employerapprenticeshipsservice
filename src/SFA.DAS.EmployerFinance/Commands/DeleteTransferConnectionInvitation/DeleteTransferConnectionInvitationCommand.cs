using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation
{
    public class DeleteTransferConnectionInvitationCommand : IAuthorizationContextModel, IAsyncRequest
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public Guid UserRef { get; set; }

        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}