using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Audit.MessageBuilders;
using SFA.DAS.EmployerAccounts.Audit.Types;

namespace SFA.DAS.EmployerAccounts.Audit;

public class AuditService : IAuditService
{
    private readonly IAuditApiClient _auditApiClient;
    private readonly IEnumerable<IAuditMessageBuilder> _builders;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IAuditApiClient auditApiClient, IEnumerable<IAuditMessageBuilder> builders, ILogger<AuditService> logger)
    {
        _auditApiClient = auditApiClient;
        _builders = builders;
        _logger = logger;
    }

    public async Task SendAuditMessage(AuditMessage message)
    {
        try
        {
            foreach (var builder in _builders)
            {
                builder.Build(message);
            }


            // ?
            message.Category = message.Category;
            message.Description = message.Description;
            message.ChangedProperties = message.ChangedProperties;
            message.RelatedEntities = message.RelatedEntities;
            message.AffectedEntity = message.AffectedEntity;

            await _auditApiClient.Audit(message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred when calling the audit service.");
        }
    }
}