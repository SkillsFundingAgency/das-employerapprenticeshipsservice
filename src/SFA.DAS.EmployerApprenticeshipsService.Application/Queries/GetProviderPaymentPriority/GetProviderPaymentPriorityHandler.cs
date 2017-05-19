using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using NLog;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority
{
    public class GetProviderPaymentPriorityHandler :
        IAsyncRequestHandler<GetProviderPaymentPriorityRequest, GetProviderPaymentPriorityResponse>
    {
        private readonly IEmployerCommitmentApi _dataLockApi;

        private readonly ILogger _logger;

        public GetProviderPaymentPriorityHandler(IEmployerCommitmentApi commitmentApi, ILogger logger)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dataLockApi = commitmentApi;
            _logger = logger;
        }

        public Task<GetProviderPaymentPriorityResponse> Handle(GetProviderPaymentPriorityRequest message)
        {
            var data = new GetProviderPaymentPriorityResponse { Data = FakePaymentPriorityStore.GetData() };

            return Task.Run(() => data);
        }

        // API things

        

        public class ProviderPaymentPriorityAPI
        {
            public IEnumerable<ProviderPaymentPriorityItemAPI> PaymentOrderItems { get; set; }
        }

        public class ProviderPaymentPriorityItemAPI
        {
            public string ProviderName { get; set; }

            public long ProviderId { get; set; }

            public int PaymentPriority { get; set; }
        }
        
    }
}