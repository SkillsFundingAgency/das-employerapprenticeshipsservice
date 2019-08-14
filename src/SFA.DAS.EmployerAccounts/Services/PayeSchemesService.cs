using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class PayeSchemesService : IPayeSchemesService
    {
        private readonly IPayeRepository _payeRepository;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly IHashingService _hashingService;

        public PayeSchemesService(
            IPayeRepository payeRepository,
            IEnglishFractionRepository englishFractionRepository,
            IHashingService hashingService
            )
        {
            _payeRepository = payeRepository;
            _englishFractionRepository = englishFractionRepository;
            _hashingService = hashingService;
        }
        public async Task<IEnumerable<PayeView>> GetPayeSchemsWithEnglishFractionForHashedAccountId(string hashedAccountId)
        {
            long accountId;

            try
            {
                accountId = _hashingService.DecodeValue(hashedAccountId);
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidRequestException(
                    new Dictionary<string, string>
                    {
                        {
                            nameof(hashedAccountId), "Hashed account ID cannot be decoded."
                        }
                    }
                );
            }

            var payeSchemes = await _payeRepository.GetPayeSchemesByAccountId(accountId);

            if (payeSchemes.Count == 0)
            {
                return payeSchemes;
            }

            await AddEnglishFractionsToPayeSchemes(accountId,
                payeSchemes);

            return payeSchemes;
        }

        private async Task AddEnglishFractionsToPayeSchemes(long accountId, List<PayeView> payeSchemes)
        {
            var englishFractions = (await _englishFractionRepository.GetCurrentFractionForSchemes(
                accountId,
                payeSchemes.Select(x => x.Ref))).Where(x => x != null).ToList();
            foreach (var scheme in payeSchemes)
            {
                scheme.EnglishFraction = englishFractions.FirstOrDefault(x => x.EmpRef == scheme.Ref);
            }
        }
    }
}