using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAllApprenticeships
{
    public class GetAllApprenticeshipsRequest : IAsyncRequest<GetAllApprenticeshipsResponse>
    {
        public long AccountId { get; set; }
    }
}
