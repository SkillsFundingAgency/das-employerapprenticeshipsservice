using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs
{
    public interface IRunOnceService
    {
        Task RunOnce(string jobName, Func<Task> function);
    }
}
