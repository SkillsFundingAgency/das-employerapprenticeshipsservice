using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerFinance.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQuery : IAuthorizationContextModel, IAsyncRequest<GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}