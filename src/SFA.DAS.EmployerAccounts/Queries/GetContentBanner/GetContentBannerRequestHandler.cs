using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetContentBanner
{
    public class GetContentBannerRequestHandler : IAsyncRequestHandler<GetContentBannerRequest, GetContentBannerResponse>
    {
        private readonly IValidator<GetContentBannerRequest> _validator;
        private readonly ILog _logger;
        private readonly IContentBannerService _service;

        public GetContentBannerRequestHandler(
            IValidator<GetContentBannerRequest> validator,
            ILog logger,
            IContentBannerService service)
        {
            _validator = validator;
            _logger = logger;
            _service = service;
        }

        public async Task<GetContentBannerResponse> Handle(GetContentBannerRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            try
            {
                return new GetContentBannerResponse
                {
                    ContentBanner = await _service.GetBannerContent(message.BannerId, message.UseCDN)
                };
            }

            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get ContentBanner for {message.BannerId}");
                return new GetContentBannerResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}
