using System;

namespace SFA.DAS.EAS.Domain.Models.UserProfile
{
    public class User
    {
        private string _userRef;
        private Guid? _externalId;

        public long Id { get; set; }

        [Obsolete("Please use 'ExternalId' instead.")]
        public string UserRef
        {
            get => _userRef ?? ExternalId.ToString();
            set => _userRef = value;
        }

        public Guid ExternalId
        {
            get => _externalId ?? Guid.Parse(_userRef);
            set => _externalId = value;
        }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
