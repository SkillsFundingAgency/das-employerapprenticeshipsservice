using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Commitments;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentsV2ApiClient _commitmentApi;

        public CommitmentService(ICommitmentsV2ApiClient commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        public async Task<List<Cohort>> GetEmployerCommitments(long employerAccountId)
        {
            //var commitmentItems = await _commitmentApi.GetEmployerCommitments(employerAccountId);
            var request = new GetCohortsRequest { AccountId = employerAccountId };
            var commitmentItems = await _commitmentApi.GetCohorts(request);

            if (commitmentItems == null || !commitmentItems.Cohorts.Any())
            {
                return new List<Cohort>();
            }

            return commitmentItems.Cohorts.Where(x => x.CommitmentStatus != CommitmentStatus.Deleted)
                .Select(x => new Cohort { Id = x.CohortId }).ToList();
        }
    }
}