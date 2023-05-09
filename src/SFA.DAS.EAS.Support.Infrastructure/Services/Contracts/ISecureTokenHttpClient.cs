namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

internal interface ISecureTokenHttpClient
{
    Task<string> GetAsync(string url);
}
