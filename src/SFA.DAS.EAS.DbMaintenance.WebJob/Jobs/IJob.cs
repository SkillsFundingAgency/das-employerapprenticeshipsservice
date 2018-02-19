using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs
{
    public interface IJob
    {
        Task Run();
    }
}