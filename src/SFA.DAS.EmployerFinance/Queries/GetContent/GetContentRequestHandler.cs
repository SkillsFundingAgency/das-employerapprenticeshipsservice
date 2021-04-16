using System;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetContent
{
    public class GetContentRequestHandler : IAsyncRequestHandler<GetContentRequest, GetContentResponse>
    {
        private readonly IValidator<GetContentRequest> _validator;
        private readonly ILog _logger;
        private readonly IContentApiClient _service;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly EmployerFinanceConfiguration _employerFinanceConfiguration;

        public GetContentRequestHandler(
            IValidator<GetContentRequest> validator,
            ILog logger,
            IContentApiClient service,
            ICacheStorageService cacheStorageService,
            EmployerFinanceConfiguration employerFinanceConfiguration
            )
        {
            _validator = validator;
            _logger = logger;
            _service = service;
            _cacheStorageService = cacheStorageService;
            _employerFinanceConfiguration = employerFinanceConfiguration;
        }

        public async Task<GetContentResponse> Handle(GetContentRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var applicationId = message.UseLegacyStyles ? _employerFinanceConfiguration.ApplicationId + "-legacy" : _employerFinanceConfiguration.ApplicationId;

            try
            {
                var content = await _service.Get(message.ContentType, applicationId);

                return new GetContentResponse
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get Content {message.ContentType} for {applicationId}");

                return new GetContentResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}
