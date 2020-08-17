using System;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
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

            var applicationId = message.UseLegacyStyles ? _employerFinanceConfiguration.ApplicationId + "-legacy" : _employerFinanceConfiguration.ApplicationId;

            try
            {
                var content = await _service.Get(message.ContentType, applicationId);

                return new GetClientContentResponse
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get Content {message.ContentType} for {applicationId}");

                return new GetClientContentResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}
