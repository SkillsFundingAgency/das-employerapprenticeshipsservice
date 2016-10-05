using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class TaskViewModel
    {
        public string HashedAccountId { get; set; }
        public Task Task { get; set; }
        public string LinkId { get; set; }
        public string Message { get; set; } 
    }
}