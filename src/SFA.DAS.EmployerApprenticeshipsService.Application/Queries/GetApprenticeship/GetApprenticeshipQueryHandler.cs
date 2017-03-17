using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryHandler : IAsyncRequestHandler<GetApprenticeshipQueryRequest, GetApprenticeshipQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;

        public GetApprenticeshipQueryHandler(IEmployerCommitmentApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetApprenticeshipQueryResponse> Handle(GetApprenticeshipQueryRequest message)
        {
            var apprenticeship = await _commitmentsApi.GetEmployerApprenticeship(message.AccountId, message.ApprenticeshipId);

            return new GetApprenticeshipQueryResponse
            {
                Apprenticeship = apprenticeship
            };
        }
    }
}