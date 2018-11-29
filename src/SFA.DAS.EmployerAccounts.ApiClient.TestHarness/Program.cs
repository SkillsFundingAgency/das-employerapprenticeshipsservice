using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.ApiClient.TestHarness.DependencyResolution;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ApiClient.TestHarness
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Task.Run(Test).Wait();

            Console.WriteLine("Press ENTER to finish");
            Console.ReadLine();
        }

        private static async Task Test()
        {
            using (var container = IoC.Initialize())
            {
                try
                {
                    var apiClient = container.GetInstance<IEmployerAccountsApiClient>();
                    var hasRoleRequest = new HasRoleRequest { EmployerAccountId = 112, UserRef = Guid.Parse("45f8e859-337c-4a4f-a184-1e794ec91f4f"), Roles = new []{ UserRole.Owner }};
                    var response = await apiClient.HasRole(hasRoleRequest, CancellationToken.None);

                    Console.WriteLine("HasRole: " + response);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }
    }
}
