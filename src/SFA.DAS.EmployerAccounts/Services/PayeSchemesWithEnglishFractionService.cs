using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiRequests.Finance;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiResponses.Finance;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.HashingService;
using DasEnglishFraction = SFA.DAS.EmployerAccounts.Models.Levy.DasEnglishFraction;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class PayeSchemesWithEnglishFractionService : IPayeSchemesWithEnglishFractionService
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly IPayeRepository _payeRepository;
        private readonly IHashingService _hashingService;
        private readonly IMapper _mapper;

        public PayeSchemesWithEnglishFractionService(IOuterApiClient outerApiClient, IPayeRepository payeRepository, IHashingService hashingService, IMapper mapper)
        {
            _outerApiClient = outerApiClient;
            _payeRepository = payeRepository;
            _hashingService = hashingService;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<PayeView>> GetPayeSchemes(string hashedAccountId)
        {
            var payeSchemes = await _payeRepository.GetPayeSchemesByAccountId(_hashingService.DecodeValue(hashedAccountId));
            if (payeSchemes.Any())
            {
                await AddEnglishFractionToPayeSchemes(hashedAccountId, payeSchemes);
            }

            return payeSchemes;
        }

        private async Task AddEnglishFractionToPayeSchemes(string hashedAccountId, IEnumerable<PayeView> payeSchemes)
        {
            var response = await _outerApiClient.Get<GetEnglishFractionCurrentResponse>(new GetEnglishFractionCurrentRequest(hashedAccountId,
                payeSchemes.Select(x => x.Ref).Where(x => !string.IsNullOrEmpty(x)).ToArray()));

            var englishFractions = _mapper.Map<List<DasEnglishFraction>>(response.Fractions);

            foreach (var scheme in payeSchemes)
            {
                scheme.EnglishFraction = englishFractions.FirstOrDefault(x => x.EmpRef == scheme.Ref);
            }
        }
    }
}