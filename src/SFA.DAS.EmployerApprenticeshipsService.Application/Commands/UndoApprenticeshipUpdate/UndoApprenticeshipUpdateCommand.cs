using MediatR;

namespace SFA.DAS.EAS.Application.Commands.UndoApprenticeshipUpdate
{
    public sealed class UndoApprenticeshipUpdateCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long AccountId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
