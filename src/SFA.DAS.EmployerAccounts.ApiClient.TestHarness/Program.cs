using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.ApiClient.TestHarness.DependencyResolution;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ApiClient.TestHarness;

public class Program
{
   public static async Task Main(string[] args)
    {
        Console.WriteLine("Employer Accounts Api Client Test Harness");

        await RunTests();

        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();
    }

    private static async Task RunTests()
    {
        using var container = IoC.Initialize();

        try
        {
            var apiClient = container.GetInstance<IEmployerAccountsApiClient>();

            await apiClient.Ping();

            Console.WriteLine("Ping succeeded");

            var isUserInRoleRequest = new IsUserInRoleRequest { AccountId = 2134, UserRef = Guid.Parse("45f8e859-337c-4a4f-a184-1e794ec91f4f"), Roles = new HashSet<UserRole>{ UserRole.Owner } } ;
            var isUserInRole = await apiClient.IsUserInRole(isUserInRoleRequest, CancellationToken.None);

            Console.WriteLine("IsUserInRole: " + isUserInRole);

            var isUserInAnyRoleRequest = new IsUserInAnyRoleRequest { AccountId = 2134, UserRef = Guid.Parse("45f8e859-337c-4a4f-a184-1e794ec91f4f") };
            var isUserInAnyRole = await apiClient.IsUserInAnyRole(isUserInAnyRoleRequest, CancellationToken.None);

            Console.WriteLine("IsUserAnyRole: " + isUserInAnyRole);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}