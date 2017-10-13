using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetFrameworks
{
    public class GetFrameworksQueryHandler : IAsyncRequestHandler<GetFrameworksQueryRequest, GetFrameworksQueryResponse>
    {
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoServiceWrapper;

        public GetFrameworksQueryHandler(IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper)
        {
            if (apprenticeshipInfoServiceWrapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipInfoServiceWrapper));
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
        }

        public async Task<GetFrameworksQueryResponse> Handle(GetFrameworksQueryRequest message)
        {
            var data = await _apprenticeshipInfoServiceWrapper.GetFrameworksAsync();

            return new GetFrameworksQueryResponse
            {
                Frameworks = data.Frameworks
            };
        }
    }
}