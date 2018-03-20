using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Core.Services;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    [ExcludeFromCodeCoverage]
    public class AccountsApiConfiguration : IAccountApiConfiguration
    {
        private readonly IProvideSettings _settings;

        public AccountsApiConfiguration(IProvideSettings settings)
        {
            _settings = settings;
        }

        public string ClientCertificateThumbprint => _settings.GetNullableSetting("AccountApiCertificateThumbprint");

        public string ApiBaseUrl => _settings.GetSetting("AccountApiBaseUrl");
        public string ClientId => _settings.GetSetting("AccountApiClientId");
        public string ClientSecret => _settings.GetSetting("AccountApiClientSecret");
        public string IdentifierUri => _settings.GetSetting("AccountApiIdentifierUri");
        public string Tenant => _settings.GetSetting("AccountApiTenant");
    }
}