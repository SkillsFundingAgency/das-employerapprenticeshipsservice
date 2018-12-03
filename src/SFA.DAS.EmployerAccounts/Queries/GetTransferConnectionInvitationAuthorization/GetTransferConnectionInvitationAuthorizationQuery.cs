using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQuery : AccountMessage, IAsyncRequest<GetTransferConnectionInvitationAuthorizationResponse>
    {
    }
}