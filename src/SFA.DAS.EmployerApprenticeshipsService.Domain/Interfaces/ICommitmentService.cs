using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Commitment;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ICommitmentService
    {
        Task<List<Cohort>> GetEmployerCommitments(long employerAccountId);
    }
}
