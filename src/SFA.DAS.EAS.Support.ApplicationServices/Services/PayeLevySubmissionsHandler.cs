using HMRC.ESFA.Levy.Api.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.NLog.Logger;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public class PayeLevySubmissionsHandler : IPayeLevySubmissionsHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILevySubmissionsRepository _levySubmissionsRepository;
        private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;
        private readonly ILogger<PayeLevySubmissionsHandler> _log;
        private readonly IPayRefHashingService _hashingService;

        public PayeLevySubmissionsHandler(IAccountRepository accountRepository,
            ILevySubmissionsRepository levySubmissionsRepository,
            IPayeSchemeObfuscator payeSchemeObfuscator,
            ILogger<PayeLevySubmissionsHandler> log,
            IPayRefHashingService hashingService)
        {
            _accountRepository = accountRepository;
            _levySubmissionsRepository = levySubmissionsRepository;
            _payeSchemeObfuscator = payeSchemeObfuscator;
            _log = log;
            _hashingService = hashingService;
        }

        public async Task<PayeLevySubmissionsResponse> FindPayeSchemeLevySubmissions(string accountId, string hashedPayeRef)
        {
            var account = await _accountRepository.Get(accountId, AccountFieldsSelection.PayeSchemes);

            if (account == null)
            {
                return new PayeLevySubmissionsResponse
                {
                    StatusCode = PayeLevySubmissionsResponseCodes.AccountNotFound
                };
            }

            var actualPayeId = _hashingService.DecodeValueToString(hashedPayeRef);

            var selectedPayeScheme = account.PayeSchemes.First(o => o.Ref.Equals(actualPayeId, StringComparison.OrdinalIgnoreCase));
            selectedPayeScheme.Ref = _payeSchemeObfuscator.ObscurePayeScheme(selectedPayeScheme.Ref);

            try
            {
                var levySubmissions = await _levySubmissionsRepository.Get(actualPayeId);

                return new PayeLevySubmissionsResponse
                {
                    StatusCode = PayeLevySubmissionsResponseCodes.Success,
                    LevySubmissions = levySubmissions ?? new LevyDeclarations(),
                    PayeScheme = selectedPayeScheme
                };
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Unable to load Levy Declarations for Account Id {accountId} ");


                return new PayeLevySubmissionsResponse
                {
                    StatusCode = PayeLevySubmissionsResponseCodes.UnexpectedError,
                    PayeScheme = selectedPayeScheme,
                };
            }
        }

    }
}