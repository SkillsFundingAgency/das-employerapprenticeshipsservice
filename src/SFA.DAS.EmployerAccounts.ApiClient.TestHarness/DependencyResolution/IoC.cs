using StructureMap;

namespace SFA.DAS.EmployerAccounts.ApiClient.TestHarness.DependencyResolution;

public static class IoC
{
    public static IContainer Initialize()
    {
        return new Container(c =>
        {
            c.AddRegistry<EmployerAccountsApiClientRegistry>();
        });
    }
}