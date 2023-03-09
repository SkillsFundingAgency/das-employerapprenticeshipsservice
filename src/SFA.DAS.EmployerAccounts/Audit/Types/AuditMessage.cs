namespace SFA.DAS.EmployerAccounts.Audit.Types;

public class AuditMessage
{
    public AuditEntity AffectedEntity { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public Source Source { get; set; }
    public List<PropertyUpdate> ChangedProperties { get; set; }
    public DateTime ChangeAt { get; set; }
    public Actor ChangedBy { get; set; }
    public List<AuditEntity> RelatedEntities { get; set; }
}