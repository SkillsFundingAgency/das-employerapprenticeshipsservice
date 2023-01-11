namespace SFA.DAS.EmployerAccounts.Models;

public class RunOnceJob : Entity
{
    public string Name { get; private set; }
    public DateTime Completed { get; private set; }

    public RunOnceJob(string name, DateTime completed)
    {
        Name = name;
        Completed = completed;
    }
}