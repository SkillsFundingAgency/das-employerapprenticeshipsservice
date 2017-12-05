using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Commitment;

namespace SFA.DAS.EAS.Infrastructure.Services
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

            return commitmentItems.Select(x => new Cohort {Id = x.Id}).ToList();
        }
    }
}
