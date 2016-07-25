using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class TeamMember
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string UserRef { get; set; }
        public int AccountId { get; set; }
        public string Role { get; set; }

        public InvitationStatus Status { get; set; }
    }
}
