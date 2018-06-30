using System;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public class UserContext : IUserContext
    {
        public long Id { get; set; }
        public Guid Ref { get; set; }
        public string Email { get; set; }
    }
}