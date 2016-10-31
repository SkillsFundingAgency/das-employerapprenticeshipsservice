﻿using SFA.DAS.EAS.LevyAccountUpdater.WebJob.DependencyResolution;
using SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob
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
