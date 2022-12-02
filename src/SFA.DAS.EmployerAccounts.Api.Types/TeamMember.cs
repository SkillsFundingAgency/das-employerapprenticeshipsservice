using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class TeamMember
    {
        public string UserRef { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public bool CanReceiveNotifications { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public InvitationStatus Status { get; set; }
    }
}
