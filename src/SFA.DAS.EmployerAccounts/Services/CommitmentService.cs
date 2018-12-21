using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Commitments;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly IEmployerCommitmentApi _commitmentApi;

        public CommitmentService(IEmployerCommitmentApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        public async Task<List<Cohort>> GetEmployerCommitments(long employerAccountId)
        {
            var commitmentItems = await _commitmentApi.GetEmployerCommitments(employerAccountId);

            if (commitmentItems == null || !commitmentItems.Any())
            {
                return new List<Cohort>();
            }

            return commitmentItems.Where(x => x.CommitmentStatus != CommitmentStatus.Deleted)
                .Select(x => new Cohort { Id = x.Id }).ToList();
        }
    }
}