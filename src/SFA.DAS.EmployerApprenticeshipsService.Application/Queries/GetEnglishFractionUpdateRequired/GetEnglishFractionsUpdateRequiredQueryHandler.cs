using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired
{
    public class GetEnglishFractionsUpdateRequiredQueryHandler : IAsyncRequestHandler<GetEnglishFractionUpdateRequiredRequest, GetEnglishFractionUpdateRequiredResponse>
    {
        private readonly IHmrcService _hmrcService;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly ICacheProvider _cacheProvider;

        public GetEnglishFractionsUpdateRequiredQueryHandler(IHmrcService hmrcService, IEnglishFractionRepository englishFractionRepository, ICacheProvider cacheProvider)
        {
            _hmrcService = hmrcService;
            _englishFractionRepository = englishFractionRepository;
            _cacheProvider = cacheProvider;
        }

        public async Task<GetEnglishFractionUpdateRequiredResponse> Handle(GetEnglishFractionUpdateRequiredRequest message)
        {
            var hmrcLatestUpdateDate = _cacheProvider.Get<DateTime?>("HmrcFractionLastCalculatedDate");

            if (hmrcLatestUpdateDate == null)
            {
                hmrcLatestUpdateDate = await _hmrcService.GetLastEnglishFractionUpdate();

                if (hmrcLatestUpdateDate != null)
                {
                    _cacheProvider.Set("HmrcFractionLastCalculatedDate", hmrcLatestUpdateDate.Value,new TimeSpan(1,0,0,0));
                }
            }

            var levyLatestUpdateDate = await _englishFractionRepository.GetLastUpdateDate();

            return new GetEnglishFractionUpdateRequiredResponse
            {
                UpdateRequired = hmrcLatestUpdateDate > levyLatestUpdateDate,
                DateCalculated = hmrcLatestUpdateDate.Value
            };
        }
    }
}
