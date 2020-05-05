using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetClientContent
{
    public class GetClientContentRequestHandler : IAsyncRequestHandler<GetClientContentRequest, GetClientContentResponse>
    {
        private readonly IValidator<GetClientContentRequest> _validator;
        private readonly ILog _logger;
        private readonly IClientContentService _service;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;

        public GetClientContentRequestHandler(
            IValidator<GetClientContentRequest> validator,
            ILog logger,
            IClientContentService service, ICacheStorageService cacheStorageService, EmployerAccountsConfiguration employerAccountsConfiguration)
        {
            _validator = validator;
            _logger = logger;
            _service = service;
            _cacheStorageService = cacheStorageService;
            _employerAccountsConfiguration = employerAccountsConfiguration;
        }

        public async Task<GetClientContentResponse> Handle(GetClientContentRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var cacheKey = _employerAccountsConfiguration.ApplicationId;

            try
            {
                if (_cacheStorageService.TryGet(cacheKey, out string cachedContentBanner))
                {
                    return new GetClientContentResponse
                    {
                        Content = cachedContentBanner
                    };
                }
                var contentBanner = await _service.GetContent(message.ContentType, cacheKey);

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
