using System;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public class UserContext : IUserContext
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Email { get; set; }
    }
}