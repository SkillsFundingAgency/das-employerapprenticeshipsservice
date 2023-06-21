using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface ITokenServiceApiClient
{
    Task<PrivilegedAccessToken> GetPrivilegedAccessTokenAsync();
}
