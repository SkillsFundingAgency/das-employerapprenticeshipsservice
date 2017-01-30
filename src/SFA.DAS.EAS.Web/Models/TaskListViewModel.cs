using System.Collections.Generic;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Models
{
    public class TaskListViewModel
    {
        public string AccountHashId { get; set; }
        public List<TaskListItemViewModel> Tasks { get; set; }
    }
}