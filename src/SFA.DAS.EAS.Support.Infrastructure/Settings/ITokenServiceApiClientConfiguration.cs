namespace SFA.DAS.EAS.Support.Infrastructure.Settings;

public interface ITokenServiceApiClientConfiguration
{
    string ApiBaseUrl { get; set; }

    string ClientId { get; set; }

    string ClientSecret { get; set; }

    string IdentifierUri { get; set; }

    string Tenant { get; set; }
}
