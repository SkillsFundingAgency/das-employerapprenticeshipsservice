using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IHmrcService
    {
        string GenerateAuthRedirectUrl(string redirectUrl);

        Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
        Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef);
    }
}