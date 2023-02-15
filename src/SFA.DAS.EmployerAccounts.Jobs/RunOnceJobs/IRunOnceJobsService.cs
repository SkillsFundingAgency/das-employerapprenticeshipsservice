namespace SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;

public interface IRunOnceJobsService
{
    Task RunOnce(string jobName, Func<Task> function);
}