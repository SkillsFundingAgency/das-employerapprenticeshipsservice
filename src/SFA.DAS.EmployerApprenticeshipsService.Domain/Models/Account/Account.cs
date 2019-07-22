using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.Account
{
    public class Account
    {
        public virtual long Id { get; set; }
        public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; } = new List<AccountLegalEntity>();
        public virtual DateTime CreatedDate { get; set; }
        public virtual string HashedId { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string PublicHashedId { get; set; }
        public virtual ICollection<TransferConnectionInvitation> ReceivedTransferConnectionInvitations { get; set; } = new List<TransferConnectionInvitation>();
        public virtual Role Role { get; set; }
        public string RoleName => ((Role)Role).ToString();
        public virtual short ApprenticeshipEmployerType { get; set; }
        public virtual ICollection<TransferConnectionInvitation> SentTransferConnectionInvitations { get; set; } = new List<TransferConnectionInvitation>();
    }
}