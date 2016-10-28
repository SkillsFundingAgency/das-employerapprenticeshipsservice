using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetUsers
{
    public class GetUsersQueryResponse
    {
        public List<User> Users { get; set; }
    }
}