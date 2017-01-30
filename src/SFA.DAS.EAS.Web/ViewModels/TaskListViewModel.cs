using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TaskListViewModel
    {
        public string AccountHashId { get; set; }
        public List<TaskListItemViewModel> Tasks { get; set; }
    }
}