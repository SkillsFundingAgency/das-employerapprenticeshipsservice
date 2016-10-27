using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyAccountUpdater.Updater
{
    public interface IAccountUpdater
    {
        Task RunUpdate();
    }
}