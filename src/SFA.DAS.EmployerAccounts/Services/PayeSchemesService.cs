﻿using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Services;

public class PayeSchemesService : IPayeSchemesService
{
    private readonly IPayeRepository _payeRepository;
    private readonly IHashingService _hashingService;

    public PayeSchemesService(IPayeRepository payeRepository, IHashingService hashingService)
    {
        _payeRepository = payeRepository;
        _hashingService = hashingService;
    }
        
    public virtual async Task<IEnumerable<PayeView>> GetPayeSchemes(string hashedAccountId)
    {
        var payeSchemes = await _payeRepository.GetPayeSchemesByAccountId(_hashingService.DecodeValue(hashedAccountId));
        return payeSchemes;
    }
}