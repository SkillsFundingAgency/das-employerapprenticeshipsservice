using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Commitments;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentsApiClient _commitmentApi;

        public CommitmentService(ICommitmentsApiClient commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        public async Task<List<Cohort>> GetEmployerCommitments(long employerAccountId)
        {          
            var commitmentItems = await _commitmentApi.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = employerAccountId }, CancellationToken.None);  //V2 call          

            if (commitmentItems == null || !commitmentItems.Cohorts.Any())            
            {
                return new List<Cohort>();
            }
            
            return commitmentItems.Cohorts.Where(x => x.CommitmentStatus != CommitmentStatus.Deleted)
                .Select(x => new Cohort { Id = x.CohortId }).ToList();
        }
    }
}