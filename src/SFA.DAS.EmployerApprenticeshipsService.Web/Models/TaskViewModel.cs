using System;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class TaskViewModel
    {
        public string LinkId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
    }
}