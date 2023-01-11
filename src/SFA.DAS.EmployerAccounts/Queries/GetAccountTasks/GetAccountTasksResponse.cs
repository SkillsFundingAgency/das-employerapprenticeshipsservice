using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;

public class GetAccountTasksResponse
{
    public ICollection<AccountTask> Tasks { get; set; } 
}