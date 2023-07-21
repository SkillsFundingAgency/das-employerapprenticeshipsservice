using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Services;

public class PayeSchemesService : IPayeSchemesService
{
    private readonly IPayeRepository _payeRepository;

    public PayeSchemesService(IPayeRepository payeRepository)
    {
        _payeRepository = payeRepository;
    }
        
    public virtual async Task<IEnumerable<PayeView>> GetPayeSchemes(long accountId)
    {
        var payeSchemes = await _payeRepository.GetPayeSchemesByAccountId(accountId);
        return payeSchemes;
    }
}