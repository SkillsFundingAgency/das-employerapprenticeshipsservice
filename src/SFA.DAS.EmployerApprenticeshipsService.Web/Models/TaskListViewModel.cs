using System.Collections.Generic;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class TaskListViewModel
    {
        public string AccountHashId { get; set; }
        public List<TaskListItemViewModel> Tasks { get; set; }
    }
}