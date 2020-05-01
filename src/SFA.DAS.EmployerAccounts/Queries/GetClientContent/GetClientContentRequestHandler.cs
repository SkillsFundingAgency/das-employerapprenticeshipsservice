using System;
using System.Threading.Tasks;
using MediatR;
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

        public GetClientContentRequestHandler(
            IValidator<GetClientContentRequest> validator,
            ILog logger,
            IClientContentService service, ICacheStorageService cacheStorageService)
        {
            _validator = validator;
            _logger = logger;
            _service = service;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<GetClientContentResponse> Handle(GetClientContentRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            try
            {
                var type = (ContentType) Enum.Parse(typeof(ContentType), message.ContentType, true);

                var cacheKey = message.ClientId;

                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    var result =
                        await _cacheStorageService.RetrieveFromCache<string>(cacheKey);

                    if (result != null)
                    {
                        return new GetClientContentResponse
                        {

                            ContentBanner = result
                        };
                    }
                }
                var cachedContentBanner = await _service.GetContentByClientId(type, message.ClientId);

                if (cachedContentBanner == null)
                {
                    throw new CachedContentBannerNotFoundException(message.ClientId);
                }

                await _cacheStorageService.SaveToCache(message.ClientId, cachedContentBanner, 1);
                
                return new GetClientContentResponse
                {

                    ContentBanner = cachedContentBanner
                };
            }

            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get ContentBanner for {message.ClientId}");

                return new GetClientContentResponse
                {
                    HasFailed = true
                };
            }
        }
    }

    [Serializable]
    public class CachedContentBannerNotFoundException : Exception
    {
        public int BannerId { get; }

        public CachedContentBannerNotFoundException() { }
        public CachedContentBannerNotFoundException(string message) : base(message) { }
        public CachedContentBannerNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected CachedContentBannerNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public CachedContentBannerNotFoundException(int bannerId)
            : base($"No reservation was found with id [{bannerId}].")
        {
            BannerId = bannerId;
        }
    }
}
