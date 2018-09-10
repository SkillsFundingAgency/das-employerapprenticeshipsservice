using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationQuery : AccountMessage, IAsyncRequest<GetTransferConnectionInvitationAuthorizationResponse>
    {
    }
}