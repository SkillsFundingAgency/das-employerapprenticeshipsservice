using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EAS.Application.Queries.GetCommitments
{
    public sealed class GetCommitmentsHandler : IAsyncRequestHandler<GetCommitmentsQuery, GetCommitmentsResponse>
    {
        private readonly ICommitmentsApi _commitmentsApi;

        public GetCommitmentsHandler(ICommitmentsApi commitmentsApi)
        {
            if(commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetCommitmentsResponse> Handle(GetCommitmentsQuery message)
        {
            var response = await _commitmentsApi.GetEmployerCommitments(message.AccountId);

            return new GetCommitmentsResponse
            {
                Commitments = response
            };
        }
    }
}
