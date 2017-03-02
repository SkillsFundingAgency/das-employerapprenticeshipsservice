using MediatR;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public Apprenticeship Apprenticeship { get; set; }

        public string UserId { get; set; }
    }
}