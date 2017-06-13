using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockQueryHandler : IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        private readonly ILog _logger;

        public GetApprenticeshipDataLockQueryHandler(
            IDataLockApi dataLockApi,
            ILog logger)
        {
            if (dataLockApi == null)
                throw new ArgumentNullException(nameof(dataLockApi));
            if (logger== null)
                throw new ArgumentNullException(nameof(logger));

            _dataLockApi = dataLockApi;
            _logger = logger;
        }

        public async Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest request)
        {
            try
            {
                var dataLockStatus = (await _dataLockApi.GetDataLocks(request.ApprenticeshipId))
                    .FirstOrDefault(m => !m.IsResolved);

                return new GetApprenticeshipDataLockResponse()
                           {
                               DataLockStatus = dataLockStatus
                           };
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Can't get apprenticeship data lock for apprenticeship {request.ApprenticeshipId}");
            }

            return new GetApprenticeshipDataLockResponse();
        }
    }
}