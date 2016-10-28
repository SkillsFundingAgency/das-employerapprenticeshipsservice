using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetStandards
{
    public class GetStandardsQueryHandler : IAsyncRequestHandler<GetStandardsQueryRequest, GetStandardsQueryResponse>
    {
        private readonly IStandardsRepository _standardsRepository;

        public GetStandardsQueryHandler(IStandardsRepository standardsRepository)
        {
            if (standardsRepository == null)
                throw new ArgumentNullException(nameof(standardsRepository));
            _standardsRepository = standardsRepository;
        }

        public async Task<GetStandardsQueryResponse> Handle(GetStandardsQueryRequest message)
        {
            var standards = await _standardsRepository.GetAllAsync();

            return new GetStandardsQueryResponse
            {
                Standards = standards.ToList()
            };
        }
    }
}