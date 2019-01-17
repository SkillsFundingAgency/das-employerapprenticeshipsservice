using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Commitments;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICommitmentService
    {
        Task<List<Cohort>> GetEmployerCommitments(long employerAccountId);
    }
}