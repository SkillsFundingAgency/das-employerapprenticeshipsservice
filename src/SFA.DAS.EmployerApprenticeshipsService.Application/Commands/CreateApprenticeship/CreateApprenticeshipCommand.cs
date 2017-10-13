using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public Apprenticeship Apprenticeship { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
    }
}