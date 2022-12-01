using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferConnectionInvitationAuthorizationResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}