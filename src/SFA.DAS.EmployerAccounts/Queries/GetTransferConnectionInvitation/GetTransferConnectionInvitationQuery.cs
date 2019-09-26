using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferConnectionInvitationResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}