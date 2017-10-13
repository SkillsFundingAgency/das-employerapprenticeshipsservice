using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockQueryHandler : IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {

        private readonly IEmployerCommitmentApi _commitmentApi;

        private readonly ILog _logger;

        public GetApprenticeshipDataLockQueryHandler(
            IEmployerCommitmentApi commitmentApi,
            ILog logger)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));
            if (logger== null)
                throw new ArgumentNullException(nameof(logger));

            _commitmentApi = commitmentApi;
            _logger = logger;
        }

        public async Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest request)
        {
            try
            {
                var dataLockStatus = (await _commitmentApi.GetDataLocks(request.AccountId, request.ApprenticeshipId))
                    .Where(m => m.Status == Status.Fail && !m.IsResolved);


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