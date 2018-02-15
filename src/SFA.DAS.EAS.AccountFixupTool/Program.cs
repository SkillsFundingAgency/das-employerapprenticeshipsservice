using System;
using System.Collections.Generic;
using SFA.DAS.EAS.AccountFixupTool.Work;

namespace SFA.DAS.EAS.AccountFixupTool
{
    internal class Program
    {
        private static List<IAdminJob> _adminJobs;

        static void Main()
        {
            _adminJobs = new List<IAdminJob>
            {
                new CreateExternalHashedAccountIdJob()
            };

            foreach (var adminJob in _adminJobs)
            {
                adminJob.Fix().Wait();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
