using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;


namespace SFA.DAS.EAS.Application.Queries.GetCharity
{
    public class GetCharityQueryHandler : IAsyncRequestHandler<GetCharityQueryRequest, GetCharityQueryResponse>
    {
        private readonly IReferenceDataService _referenceDataService;

        public GetCharityQueryHandler(IReferenceDataService referenceDataService)
        {
            _referenceDataService = referenceDataService;
        }

        public async Task<GetCharityQueryResponse> Handle(GetCharityQueryRequest message)
        {
            var charity = await _referenceDataService.GetCharity(message.RegistrationNumber);

            return new GetCharityQueryResponse
            {
                Charity = charity
            };
        }

    }
}
