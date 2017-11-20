﻿namespace SFA.DAS.EAS.Domain.Models.UserProfile
{
    public class User
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserRef { get; set; }

        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
