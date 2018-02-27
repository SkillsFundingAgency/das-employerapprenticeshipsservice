using System;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class UserDto
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}