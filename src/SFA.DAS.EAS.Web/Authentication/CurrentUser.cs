using System;

namespace SFA.DAS.EAS.Web.Authentication
{
    public class CurrentUser
    {
        public Guid ExternalId { get; set; }
        public string Email { get; set; }
    }
}