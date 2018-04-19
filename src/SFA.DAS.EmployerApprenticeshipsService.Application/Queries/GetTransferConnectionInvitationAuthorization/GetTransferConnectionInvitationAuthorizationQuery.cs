using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQuery : AccountMessage, IAsyncRequest<GetTransferConnectionInvitationAuthorizationResponse>
    {
    }
}