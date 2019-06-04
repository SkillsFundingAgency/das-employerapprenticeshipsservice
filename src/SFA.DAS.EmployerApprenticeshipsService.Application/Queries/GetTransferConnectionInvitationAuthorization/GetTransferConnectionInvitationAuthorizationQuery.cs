using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQuery : AccountMessage, IAsyncRequest<GetTransferConnectionInvitationAuthorizationResponse>
    {
    }
}