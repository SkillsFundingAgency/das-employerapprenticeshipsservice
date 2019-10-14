using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}