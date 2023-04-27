using SFA.DAS.Audit.Types;

namespace SFA.DAS.EAS.Domain.Models.Audit;

public class EasAuditMessage
{
    public List<PropertyUpdate> ChangedProperties { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public List<DAS.Audit.Types.Entity> RelatedEntities { get; set; }
    public DAS.Audit.Types.Entity AffectedEntity { get; set; }
}