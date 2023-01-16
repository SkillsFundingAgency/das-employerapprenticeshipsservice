using System.Threading;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;

public class GetProviderInvitationQueryHandler : IRequestHandler<GetProviderInvitationQuery, GetProviderInvitationResponse>
{
    private readonly IProviderRegistrationApiClient _providerRegistrationApiClient;
    private readonly ILog _logger;

    public GetProviderInvitationQueryHandler(IProviderRegistrationApiClient providerRegistrationApiClient, ILog logger)
    {
        _providerRegistrationApiClient = providerRegistrationApiClient;
        _logger = logger;
    }

    public async Task<GetProviderInvitationResponse> Handle(GetProviderInvitationQuery message, CancellationToken cancellationToken)
    {
        _logger.Info($"Get Invitations for {message.CorrelationId}");
        var json = await _providerRegistrationApiClient.GetInvitations(message.CorrelationId.ToString());
        _logger.Info($"Request sent Get Invitations for {message.CorrelationId} {json}");
        return new GetProviderInvitationResponse
        {
            Result = json == null ? null : JsonConvert.DeserializeObject<ProviderInvitation>(json)
        };
    }
}