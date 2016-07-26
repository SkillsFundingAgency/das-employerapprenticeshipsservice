using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember
{
    public class GetMemberQueryHandler : IAsyncRequestHandler<GetMemberRequest, GetMemberResponse>
    {

        public Task<GetMemberResponse> Handle(GetMemberRequest message)
        {
            throw new System.NotImplementedException();
        }
    }
}