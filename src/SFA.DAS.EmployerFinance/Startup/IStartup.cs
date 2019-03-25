using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Startup
{
    public interface IStartup
    {
        Task StartAsync();
        Task StopAsync();
    }
}
