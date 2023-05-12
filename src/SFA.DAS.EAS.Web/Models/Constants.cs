using SFA.DAS.Authentication;

namespace SFA.DAS.EAS.Web.Models;

public class Constants
{
    private readonly IdentityServerConfiguration _configuration;

    public Constants(IdentityServerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string ChangeEmailLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangeEmailLink, _configuration.ClientId);
    public string ChangePasswordLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangePasswordLink, _configuration.ClientId);
}
