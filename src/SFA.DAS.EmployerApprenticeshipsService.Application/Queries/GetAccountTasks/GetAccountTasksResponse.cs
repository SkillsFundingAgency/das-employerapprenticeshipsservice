using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTasks
{
    public class GetAccountTasksResponse
    {
        public ICollection<AccountTask> Tasks { get; set; } 
    }
}
