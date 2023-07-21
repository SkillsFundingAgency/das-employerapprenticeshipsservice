namespace SFA.DAS.EmployerAccounts.Web;

public class Constants
{
    private readonly IdentityServerConfiguration _configuration;

    public Constants(IdentityServerConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string LogoutEndpoint() => $"{_configuration.BaseAddress}{_configuration.LogoutEndpoint}";
    public string RegisterLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.RegisterLink, _configuration.ClientId);
}