using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Application.Queries.GetAllApprenticeships
{
    public class GetAllApprenticeshipsHandler : IAsyncRequestHandler<GetAllApprenticeshipsRequest, GetAllApprenticeshipsResponse>
    {
        private readonly ICommitmentsApi _commitmentsApi;

        public GetAllApprenticeshipsHandler(ICommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetAllApprenticeshipsResponse> Handle(GetAllApprenticeshipsRequest message)
        {
            var apprenticeship = await _commitmentsApi.GetEmployerApprenticeships(message.AccountId);
            return new GetAllApprenticeshipsResponse
                       {
                           Apprenticeships =
                               apprenticeship.Where(m => m
                               .PaymentStatus != PaymentStatus.PendingApproval)
                               .ToList()
                       };
        }
    }
}