using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Commands.UpdateApprenticeship
{
    public class UpdateApprenticeshipCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public Apprenticeship Apprenticeship { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}