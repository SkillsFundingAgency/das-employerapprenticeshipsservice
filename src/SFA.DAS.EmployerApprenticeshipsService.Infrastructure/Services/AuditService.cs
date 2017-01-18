using System;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Audit.Client;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditApiClient _auditApiClient;
        private readonly IAuditMessageFactory _factory;
        private readonly ILogger _logger;

        public AuditService(IAuditApiClient auditApiClient, IAuditMessageFactory factory, ILogger logger)
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
                auditMessage.Description = message.Description;
                auditMessage.ChangedProperties = message.ChangedProperties;
                auditMessage.RelatedEntities = message.RelatedEntities;
                auditMessage.AffectedEntity = message.AffectedEntity;

                await _auditApiClient.Audit(auditMessage);
            }
            catch (Exception exception)
            {
                _logger.Error(exception,"An error occurred when calling the audit service.");
            }
        }
    }
}
