using System.Threading;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetContent;

public class GetContentRequestHandler : IRequestHandler<GetContentRequest, GetContentResponse>
{
    private readonly IValidator<GetContentRequest> _validator;
    private readonly ILog _logger;
    private readonly IContentApiClient _contentApiClient;
    private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;

    public GetContentRequestHandler(
        IValidator<GetContentRequest> validator,
        ILog logger,
        IContentApiClient contentApiClient,  EmployerAccountsConfiguration employerAccountsConfiguration)
    {
        _validator = validator;
        _logger = logger;
        _contentApiClient = contentApiClient;
        _employerAccountsConfiguration = employerAccountsConfiguration;
    }

    public async Task<GetContentResponse> Handle(GetContentRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var applicationId = message.UseLegacyStyles? _employerAccountsConfiguration.ApplicationId + "-legacy" : _employerAccountsConfiguration.ApplicationId;            

        try
        {                
            var contentBanner = await _contentApiClient.Get(message.ContentType, applicationId);

            return new GetContentResponse
            {
                Content = contentBanner
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