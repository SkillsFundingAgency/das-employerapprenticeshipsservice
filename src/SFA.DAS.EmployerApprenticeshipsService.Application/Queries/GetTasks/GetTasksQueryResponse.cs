using System.Collections.Generic;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks
{
    public class GetTasksQueryResponse
    {
        public List<Task> Tasks { get; set; }
    }
}