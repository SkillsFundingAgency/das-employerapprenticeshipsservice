using SFA.DAS.EAS.LevyAccountUpdater.DependencyResolution;
using SFA.DAS.EAS.LevyAccountUpdater.Updater;

namespace SFA.DAS.EAS.LevyAccountUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = IoC.Initialize();

            var updater = container.GetInstance<IAccountUpdater>();

            updater.RunUpdate().Wait();
        }
    }
}
