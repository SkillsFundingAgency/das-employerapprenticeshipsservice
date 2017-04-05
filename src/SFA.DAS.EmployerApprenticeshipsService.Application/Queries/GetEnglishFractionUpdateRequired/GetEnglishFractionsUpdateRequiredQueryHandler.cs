using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired
{
    public class GetEnglishFractionsUpdateRequiredQueryHandler : IAsyncRequestHandler<GetEnglishFractionUpdateRequiredRequest, GetEnglishFractionUpdateRequiredResponse>
    {
        private readonly IHmrcService _hmrcService;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        

        public GetEnglishFractionsUpdateRequiredQueryHandler(IHmrcService hmrcService, IEnglishFractionRepository englishFractionRepository)
        {
            _hmrcService = hmrcService;
            _englishFractionRepository = englishFractionRepository;
            
        }

        public async Task<GetEnglishFractionUpdateRequiredResponse> Handle(GetEnglishFractionUpdateRequiredRequest message)
        {
            var hmrcLatestUpdateDate = await _hmrcService.GetLastEnglishFractionUpdate();
            var levyLatestUpdateDate = await _englishFractionRepository.GetLastUpdateDate();

            return new GetEnglishFractionUpdateRequiredResponse
            {
                UpdateRequired = hmrcLatestUpdateDate > levyLatestUpdateDate,
                DateCalculated = hmrcLatestUpdateDate
            };
        }
    }
}
