using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetSentTransferConnectionInvitation
{
    public class GetSentTransferConnectionInvitationQuery : IAuthorizationContextModel, IAsyncRequest<GetSentTransferConnectionInvitationResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}