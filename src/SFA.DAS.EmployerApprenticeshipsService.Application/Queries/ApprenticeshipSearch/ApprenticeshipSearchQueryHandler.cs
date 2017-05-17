using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryHandler : IAsyncRequestHandler<ApprenticeshipSearchQueryRequest, ApprenticeshipSearchQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IHashingService _hashingService;

        public ApprenticeshipSearchQueryHandler(IEmployerCommitmentApi employerCommitmentsApi, IHashingService hashingService)
        {
            if (employerCommitmentsApi == null)
                throw new ArgumentNullException(nameof(employerCommitmentsApi));

            if(hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _commitmentsApi = employerCommitmentsApi;
            _hashingService = hashingService;
        }

        public async Task<ApprenticeshipSearchQueryResponse> Handle(ApprenticeshipSearchQueryRequest message)
        {
            var accountId = _hashingService.DecodeValue(message.HashedLegalEntityId);

            var data = await _commitmentsApi.GetEmployerApprenticeships(accountId, message.Query);

            return new ApprenticeshipSearchQueryResponse
            {
                Apprenticeships = data.Apprenticeships.ToList(),
                Facets = data.Facets
            };
        }
    }
}
