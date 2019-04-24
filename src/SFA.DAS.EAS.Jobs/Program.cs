using Microsoft.Extensions.Hosting;

namespace SFA.DAS.EAS.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
            });
            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }
    }
}
