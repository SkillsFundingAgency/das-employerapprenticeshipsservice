using MediatR;

namespace SFA.DAS.EAS.Application.Commands.DeleteApprentice
{
    public class DeleteApprenticeshipCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public long ApprenticeshipId { get; set; }

        public string UserId { get; set; }
    }
}
