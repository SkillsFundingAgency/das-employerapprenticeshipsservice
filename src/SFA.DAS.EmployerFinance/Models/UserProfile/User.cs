using System;

namespace SFA.DAS.EmployerFinance.Models.UserProfile
{
    public class User
    {
        public virtual long Id { get; set; }

        public virtual Guid Ref
        {
            get => _ref ?? Guid.Parse(_userRef);
            set => _ref = value;
        }

        [Obsolete("Please use 'Ref' instead.")]
        public string UserRef
        {
            get => _userRef ?? _ref.Value.ToString();
            set => _userRef = value;
        }

        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string CorrelationId { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        
        private Guid? _ref;
        private string _userRef;
    }
}
