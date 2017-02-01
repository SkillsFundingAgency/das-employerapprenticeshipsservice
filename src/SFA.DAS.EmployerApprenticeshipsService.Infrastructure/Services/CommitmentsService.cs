using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class CommitmentsService : ICommitmentsService
    {
        private readonly ICommitmentsApi _commitmentsApi;

        public CommitmentsService(ICommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            await _commitmentsApi.DeleteEmployerApprenticeship(employerAccountId, apprenticeshipId);
        }
    }
}
