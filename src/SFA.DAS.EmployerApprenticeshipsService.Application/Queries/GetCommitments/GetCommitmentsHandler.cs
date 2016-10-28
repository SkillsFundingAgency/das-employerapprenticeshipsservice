using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetCommitments
{
    public sealed class GetCommitmentsHandler : IAsyncRequestHandler<GetCommitmentsQuery, GetCommitmentsResponse>
    {
        private readonly ICommitmentsApi _commitmentsApi;
        private readonly IHashingService _hashingService;

        public GetCommitmentsHandler(ICommitmentsApi commitmentsApi, IHashingService hashingService)
        {
            if(commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if(hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _commitmentsApi = commitmentsApi;
            _hashingService = hashingService;
        }

        public async Task<GetCommitmentsResponse> Handle(GetCommitmentsQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashId);
            var response = await _commitmentsApi.GetEmployerCommitments(accountId);

            return new GetCommitmentsResponse
            {
                Commitments = response
            };
        }
    }
}
