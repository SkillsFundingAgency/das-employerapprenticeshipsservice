using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockQueryHandler : IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        public GetApprenticeshipDataLockQueryHandler(IDataLockApi dataLockApi)
        {
            if (dataLockApi == null)
                throw new ArgumentNullException(nameof(dataLockApi));

            _dataLockApi = dataLockApi;
        }

        public async Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest request)
        {
            var dataLockStatus = (await _dataLockApi.GetDataLocks(request.ApprenticeshipId)).FirstOrDefault();

            return new GetApprenticeshipDataLockResponse()
                       {
                           DataLockStatus = dataLockStatus
                       };
        }
    }
}