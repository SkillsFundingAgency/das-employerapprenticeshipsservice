using HMRC.ESFA.Levy.Api.Types;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Interfaces.Hmrc;

public interface IHmrcService
{
    string GenerateAuthRedirectUrl(string redirectUrl);
    Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
    Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef);
    Task<string> DiscoverEmpref(string authToken);
}