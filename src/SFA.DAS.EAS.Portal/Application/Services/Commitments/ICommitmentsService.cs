using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments
{
    public interface ICommitmentsService
    {
        Task<CommitmentView> GetProviderCommitment(
            long providerId,
            long commitmentId,
            CancellationToken cancellationToken = default);
    }
}