using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipUpdate
{
    public class GetApprenticeshipUpdateRequest : IAsyncRequest<GetApprenticeshipUpdateResponse>
    {
        public long AccountId { get; set; }

        public long ApprenticehsipId { get; set; }
    }
}
