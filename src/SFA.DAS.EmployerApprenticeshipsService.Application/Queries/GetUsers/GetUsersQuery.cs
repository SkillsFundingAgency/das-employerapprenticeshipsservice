using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers
{
    public class GetUsersQuery : IAsyncRequest<List<User>>
    {
    }
}
