using AutoMapper;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Finance;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Finance;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.Encoding;
using DasEnglishFraction = SFA.DAS.EmployerAccounts.Models.Levy.DasEnglishFraction;

namespace SFA.DAS.EmployerAccounts.Services;

public class PayeSchemesWithEnglishFractionService : IPayeSchemesWithEnglishFractionService
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly IPayeRepository _payeRepository;
    private readonly IEncodingService _encodingService;
    private readonly IMapper _mapper;

    public PayeSchemesWithEnglishFractionService(IOuterApiClient outerApiClient, IPayeRepository payeRepository, IEncodingService encodingService, IMapper mapper)
    {
        _outerApiClient = outerApiClient;
        _payeRepository = payeRepository;
        _encodingService = encodingService;
        _mapper = mapper;
    }
        
    public async Task<IEnumerable<PayeView>> GetPayeSchemes(long accountId)
    {
        var payeSchemes = await _payeRepository.GetPayeSchemesByAccountId(accountId);
        if (payeSchemes.Any())
        {
            await AddEnglishFractionToPayeSchemes(accountId, payeSchemes);
        }

        return payeSchemes;
    }

    private async Task AddEnglishFractionToPayeSchemes(long accountId, IEnumerable<PayeView> payeSchemes)
    {
        var hashedAccountId = _encodingService.Encode(accountId, EncodingType.AccountId);

        // TODO: Outer API should use long accountId
        var response = await _outerApiClient.Get<GetEnglishFractionCurrentResponse>(new GetEnglishFractionCurrentRequest(hashedAccountId, payeSchemes.Select(x => x.Ref).Where(x => !string.IsNullOrEmpty(x)).ToArray()));

        var englishFractions = _mapper.Map<List<DasEnglishFraction>>(response.Fractions);

        foreach (var scheme in payeSchemes)
        {
            scheme.EnglishFraction = englishFractions.FirstOrDefault(x => x.EmpRef == scheme.Ref);
        }
    }
}