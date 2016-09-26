using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class TaskViewModel
    {
        public long AccountId { get; set; }
        public Task Task { get; set; }
        public long LinkId { get; set; }
        public string Message { get; set; } 
    }
}