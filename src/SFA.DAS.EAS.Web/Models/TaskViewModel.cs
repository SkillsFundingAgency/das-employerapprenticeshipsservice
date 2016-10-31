using System;

namespace SFA.DAS.EAS.Web.Models
{
    public class TaskViewModel
    {
        public string LinkId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
    }
}