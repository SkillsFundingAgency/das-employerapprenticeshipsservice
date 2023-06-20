namespace SFA.DAS.EmployerAccounts.TasksApi;

public class TaskDto
{
    public string Type { get; set; }
    public string EmployerAccountId { get; set; }
    public int ItemsDueCount { get; set; }

    public string OwnerId => EmployerAccountId;
}