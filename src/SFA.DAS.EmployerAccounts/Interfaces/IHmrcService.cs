using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.HmrcLevy;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IHmrcService
    {
        string GenerateAuthRedirectUrl(string redirectUrl);

        Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
        Task<HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef);
        Task<string> DiscoverEmpref(string authToken);
    }
}