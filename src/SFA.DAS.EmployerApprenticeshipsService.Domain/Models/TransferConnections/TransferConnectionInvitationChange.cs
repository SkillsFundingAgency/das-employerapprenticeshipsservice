using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.TransferConnections;

public class TransferConnectionInvitationChange
{
    public virtual int Id { get; protected set; }
    public virtual DateTime CreatedDate { get; protected internal set; }
    public virtual bool? DeletedByReceiver { get; protected internal set; }
    public virtual bool? DeletedBySender { get; protected internal set; }
    public virtual Account.Account ReceiverAccount { get; protected internal set; }
    public virtual long? ReceiverAccountId { get; protected set; }
    public virtual Account.Account SenderAccount { get; protected internal set; }
    public virtual long? SenderAccountId { get; protected set; }
    public virtual TransferConnectionInvitationStatus? Status { get; protected internal set; }
    public virtual int TransferConnectionInvitationId { get; protected set; }
    public virtual User User { get; protected internal set; }
    public virtual long UserId { get; protected set; }

    protected internal TransferConnectionInvitationChange()
    {
    }
}