using System;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class TaskListItemViewModel
    {
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
        public TaskStatuses Status { get; set; }
        public string HashedTaskId { get; set; }
    }
}