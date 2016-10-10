using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHashedQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public string HashedId { get; set; }
        public string UserId { get; set; }
    }
}
