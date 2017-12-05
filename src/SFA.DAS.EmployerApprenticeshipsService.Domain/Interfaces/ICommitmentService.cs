using System.Collections.Generic;
using System.Threading.Tasks;
using Commitment = SFA.DAS.EAS.Domain.Models.Commitment.Commitment;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ICommitmentService
    {
        Task<List<Commitment>> GetEmployerCommitments(long employerAccountId);
    }
}
