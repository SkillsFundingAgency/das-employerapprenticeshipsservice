﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryHandler : IAsyncRequestHandler<GetApprenticeshipQueryRequest, GetApprenticeshipQueryResponse>
    {
        private readonly ICommitmentsApi _commitmentsApi;

        public GetApprenticeshipQueryHandler(ICommitmentsApi commitmentsApi)
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