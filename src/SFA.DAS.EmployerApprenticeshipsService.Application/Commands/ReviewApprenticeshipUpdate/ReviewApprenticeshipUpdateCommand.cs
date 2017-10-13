using MediatR;

namespace SFA.DAS.EAS.Application.Commands.ReviewApprenticeshipUpdate
{
    public class ReviewApprenticeshipUpdateCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long AccountId { get; set; }
        public long ApprenticeshipId { get; set; }
        public bool IsApproved { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
    }
}