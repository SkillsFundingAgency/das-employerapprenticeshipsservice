using System;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority
{
    public class GetProviderPaymentPriorityHandler :
        IAsyncRequestHandler<GetProviderPaymentPriorityRequest, GetProviderPaymentPriorityResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;

        public GetProviderPaymentPriorityHandler(IEmployerCommitmentApi commitmentApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));

            _commitmentApi = commitmentApi;
        }

        public async Task<GetProviderPaymentPriorityResponse> Handle(GetProviderPaymentPriorityRequest message)
        {
            var data = await _commitmentApi.GetCustomProviderPaymentPriority(message.AccountId);   

            return new GetProviderPaymentPriorityResponse { Data = data };
        }
    }
}