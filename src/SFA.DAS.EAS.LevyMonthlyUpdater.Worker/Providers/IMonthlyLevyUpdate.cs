using System.Threading.Tasks;

namespace WorkerRole1.Providers
{
    interface IMonthlyLevyUpdate
    {
        Task Handle();
    }
}
