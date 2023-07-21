using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;

public class GetProviderInvitationQueryHandler : IRequestHandler<GetProviderInvitationQuery, GetProviderInvitationResponse>
{
    private readonly IProviderRegistrationApiClient _providerRegistrationApiClient;
    private readonly ILogger<GetProviderInvitationQueryHandler> _logger;

    public GetProviderInvitationQueryHandler(IProviderRegistrationApiClient providerRegistrationApiClient, ILogger<GetProviderInvitationQueryHandler> logger)
    {
        _providerRegistrationApiClient = providerRegistrationApiClient;
        _logger = logger;
    }

    public async Task<GetProviderInvitationResponse> Handle(GetProviderInvitationQuery message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get Invitations for {message.CorrelationId}", message.CorrelationId);

        var json = await _providerRegistrationApiClient.GetInvitations(message.CorrelationId.ToString());

        _logger.LogInformation("Request sent Get Invitations for {CorrelationId} {Json}", message.CorrelationId, json);

        return new GetProviderInvitationResponse
        {
            Result = json == null ? null : JsonConvert.DeserializeObject<ProviderInvitation>(json)
        };
    }
}