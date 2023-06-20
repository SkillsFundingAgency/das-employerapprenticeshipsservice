namespace SFA.DAS.EmployerAccounts.Startup;

public interface IStartup
{
    Task StartAsync();
    Task StopAsync();
}