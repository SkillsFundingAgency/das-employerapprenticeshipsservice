using NUnit.Framework;
using SFA.DAS.Testing.AzureStorageEmulator;

namespace SFA.DAS.EmployerAccounts.IntegrationTests;

[SetUpFixture]
public class TestSetup
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        AzureStorageEmulatorManager.StartStorageEmulator();
    }
}