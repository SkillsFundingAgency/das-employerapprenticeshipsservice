using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Worker.Jobs
{
    public interface IJob
    {
        Task Run();
    }
}