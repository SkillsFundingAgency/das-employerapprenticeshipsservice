using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Queries.GetUsers
{
    public class GetUsersQueryResponse
    {
        public List<User> Users { get; set; }
    }
}