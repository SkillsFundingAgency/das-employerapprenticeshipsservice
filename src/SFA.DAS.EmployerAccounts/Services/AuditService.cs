using Microsoft.Extensions.Logging;
using SFA.DAS.Audit.Client;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Services;

public class AuditService : IAuditService
{
    private readonly IAuditApiClient _auditApiClient;
    private readonly IAuditMessageFactory _factory;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IAuditApiClient auditApiClient, IAuditMessageFactory factory, ILogger<AuditService> logger)
    {
        _auditApiClient = auditApiClient;
        _factory = factory;
        _logger = logger;
    }

    public async Task SendAuditMessage(EasAuditMessage message)
    {
        try
        {
            var auditMessage = _factory.Build();
            auditMessage.Category = message.Category;
            auditMessage.Description = message.Description;
            auditMessage.ChangedProperties = message.ChangedProperties;
            auditMessage.RelatedEntities = message.RelatedEntities;
            auditMessage.AffectedEntity = message.AffectedEntity;

            await _auditApiClient.Audit(auditMessage);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"An error occurred when calling the audit service.");
        }
    }
}