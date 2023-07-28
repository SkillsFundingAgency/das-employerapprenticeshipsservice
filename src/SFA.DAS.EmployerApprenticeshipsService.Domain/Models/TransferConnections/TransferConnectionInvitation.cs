namespace SFA.DAS.EAS.Domain.Models.TransferConnections;

/// <summary>
/// This class has to remain because the API uses it, and the other transfers related classes, for queries.  Once those queries have been moved to EmployerAccounts this class, and all
/// other transfers classes, can be deleted.
/// </summary>
public class TransferConnectionInvitation 
{
    public virtual int Id { get; protected set; }
    public virtual ICollection<TransferConnectionInvitationChange> Changes { get; protected set; } = new List<TransferConnectionInvitationChange>();
    public virtual DateTime CreatedDate { get; protected set; }
    public virtual bool DeletedByReceiver { get; protected set; }
    public virtual bool DeletedBySender { get; protected set; }
    public virtual Account.Account ReceiverAccount { get; protected set; }
    public virtual long ReceiverAccountId { get; protected set; }
    public virtual Account.Account SenderAccount { get; protected set; }
    public virtual long SenderAccountId { get; protected set; }
    public virtual TransferConnectionInvitationStatus Status { get; protected set; }

    protected TransferConnectionInvitation()
    {
    }
}