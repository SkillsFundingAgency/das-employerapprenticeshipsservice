using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
        [Required]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$")]
        public string HashedAccountId { get; set; }
    }
}