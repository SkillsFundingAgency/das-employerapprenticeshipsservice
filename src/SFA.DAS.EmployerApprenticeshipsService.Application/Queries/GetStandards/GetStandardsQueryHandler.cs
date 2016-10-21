using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetStandards
{
    public class GetStandardsQueryHandler : IAsyncRequestHandler<GetStandardsQueryRequest, GetStandardsQueryResponse>
    {
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoServiceWrapper;

        public GetStandardsQueryHandler(IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper)
        {
            if (apprenticeshipInfoServiceWrapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipInfoServiceWrapper));
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
        }

        public async Task<GetStandardsQueryResponse> Handle(GetStandardsQueryRequest message)
        {
            var data = await _apprenticeshipInfoServiceWrapper.GetStandardsAsync();

            return new GetStandardsQueryResponse
            {
                Standards = data.Standards
            };
        }
    }
}