namespace SFA.DAS.EAS.Domain.Configuration;

public static class ConfigurationKeys
{
    public const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";

    public static string EmployerApprenticeshipsService => ServiceName;
    public const string AzureActiveDirectoryApiConfiguration = "AzureADApiAuthentication";
    public const string Identity = $"{ServiceName}:Identity";
}
