using System;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class UserViewModel
    {
        public long Id { get; set; }
        public Guid ExternalUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserSelected { get; set; }
    }
}