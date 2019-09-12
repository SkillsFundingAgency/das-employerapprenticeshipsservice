using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetReceivedTransferConnectionInvitation
{
    public class GetReceivedTransferConnectionInvitationQuery : IAuthorizationContextModel, IAsyncRequest<GetReceivedTransferConnectionInvitationResponse>
    {
        [Required]
        public long AccountId { get; set; }

        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}