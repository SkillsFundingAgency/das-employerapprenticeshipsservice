using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers
{
    public class GetUsersQueryResponse
    {
        public List<User> Users { get; set; }
    }
}