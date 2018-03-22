using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTransferRole
{
    public class GetAccountTransferRoleQuery : AuthorizedMessage, IAsyncRequest<GetAccountTransferRoleResponse>
    {
    }
}