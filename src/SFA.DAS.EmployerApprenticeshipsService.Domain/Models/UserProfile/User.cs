using System;
using System.Collections;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Domain.Models.UserProfile
{
    public class User
    {
        public virtual long Id { get; set; }

        public virtual Guid ExternalId
        {
            get => _externalId ?? Guid.Parse(_userRef);
            set => _externalId = value;
        }

        [Obsolete("Please use 'ExternalId' instead.")]
        public string UserRef
        {
            get => _userRef ?? _externalId.Value.ToString();
            set => _userRef = value;
        }

        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public virtual ICollection<UserAccountSetting> UserAccountSettings { get;  protected set; } = new List<UserAccountSetting>();

        public virtual ICollection<Membership> Memberships { get; protected set; } = new List<Membership>();

        private Guid? _externalId;
        private string _userRef;
    }
}
