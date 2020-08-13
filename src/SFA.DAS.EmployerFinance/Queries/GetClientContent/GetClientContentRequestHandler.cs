using System;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
//using SFA.DAS.EmployerAccounts.Configuration;
//using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetClientContent
{
    public class GetClientContentRequestHandler : IAsyncRequestHandler<GetClientContentRequest, GetClientContentResponse>
    {
        private readonly IValidator<GetClientContentRequest> _validator;
        private readonly ILog _logger;
        private readonly IClientContentService _service;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly EmployerFinanceConfiguration _employerFinanceConfiguration;

        public GetClientContentRequestHandler(
            IValidator<GetClientContentRequest> validator,
            ILog logger,
            IClientContentService service,
            ICacheStorageService cacheStorageService,
            //EmployerAccountsConfiguration employerAccountsConfiguration)
            EmployerFinanceConfiguration employerFinanceConfiguration
            )
        {
            _validator = validator;
            _logger = logger;
            _service = service;
            _cacheStorageService = cacheStorageService;
            _employerFinanceConfiguration = employerFinanceConfiguration;
        }

        public async Task<GetClientContentResponse> Handle(GetClientContentRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            //var applicationId = message.UseLegacyStyles ? _employerFinanceConfiguration.ApplicationId + "-legacy" : _employerFinanceConfiguration.ApplicationId;

            //var applicationId = message.UseLegacyStyles ? "das-employeraccounts-web-legacy" : "das-employeraccounts-web";
            var applicationId = message.UseLegacyStyles ? "das-employerfinance-web-legacy" : "das-employerfinance-web";

            var cacheKey = $"{applicationId}_{message.ContentType}".ToLowerInvariant();

            try
            {
                if (_cacheStorageService.TryGet(cacheKey, out string cachedContentBanner))
                {
                    return new GetClientContentResponse
                    {
                        Content = cachedContentBanner
                    };
                }

                var contentBanner = await _service.Get(message.ContentType, applicationId);

                if (contentBanner != null)
                {
                    await _cacheStorageService.Save(cacheKey, contentBanner, 1);
                }
                return new GetClientContentResponse
                {
                    Content = contentBanner
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get Content for {cacheKey}");

                return new GetClientContentResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}
