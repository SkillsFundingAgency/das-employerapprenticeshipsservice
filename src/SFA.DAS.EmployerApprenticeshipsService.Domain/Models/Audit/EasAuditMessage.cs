using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EAS.Domain.Models.Audit
{
    public class EasAuditMessage
    {
        public List<PropertyUpdate> ChangedProperties { get; set; }
        public string Description { get; set; }
        public List<Entity> RelatedEntities { get; set; }
        public Entity AffectedEntity { get; set; }
    }
}