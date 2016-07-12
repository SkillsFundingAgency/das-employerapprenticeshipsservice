using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQuery : IAsyncRequest<GetUserAccountsQueryResponse>
    {
        public string UserId { get; set; }
    }
}
