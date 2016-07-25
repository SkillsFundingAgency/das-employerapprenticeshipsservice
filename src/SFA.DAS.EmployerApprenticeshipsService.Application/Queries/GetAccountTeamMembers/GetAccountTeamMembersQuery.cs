using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersQuery : IAsyncRequest<GetAccountTeamMembersResponse>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }
}


