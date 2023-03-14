using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Models;

public class Constants
{
    private readonly string _baseUrl;
    private readonly IdentityServerConfiguration _configuration;

    public Constants(IdentityServerConfiguration configuration)
    {
        _baseUrl = configuration.ClaimIdentifierConfiguration.ClaimsBaseUrl;
        _configuration = configuration;
    }

    public string AuthorizeEndpoint() => $"{_configuration.BaseAddress}{_configuration.AuthorizeEndPoint}";
    public string ChangeEmailLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangeEmailLink, _configuration.ClientId);
    public string ChangePasswordLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangePasswordLink, _configuration.ClientId);
    public string DisplayName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.DisplayName;
    public string Email() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Email;
    public string FamilyName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.FaimlyName;
    public string GivenName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.GivenName;
    public string Id() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Id;
    public string LogoutEndpoint() => $"{_configuration.BaseAddress}{_configuration.LogoutEndpoint}";
    public string RegisterLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.RegisterLink, _configuration.ClientId);
    public string RequiresVerification() => _baseUrl + "requires_verification";
    public string TokenEndpoint() => $"{_configuration.BaseAddress}{_configuration.TokenEndpoint}";
    public string UserInfoEndpoint() => $"{_configuration.BaseAddress}{_configuration.UserInfoEndpoint}";
}
