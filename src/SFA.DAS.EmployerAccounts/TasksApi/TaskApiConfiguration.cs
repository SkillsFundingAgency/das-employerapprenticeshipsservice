namespace SFA.DAS.EmployerAccounts.TasksApi;

public interface ITaskApiConfiguration
{
    string ApiBaseUrl { get; }
    string ClientId { get; }
    string ClientSecret { get; }
    string IdentifierUri { get; }
    string Tenant { get; }
}


public class TaskApiConfiguration : ITaskApiConfiguration
{
    public string ApiBaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string IdentifierUri { get; set; }
    public string Tenant { get; set; }
}