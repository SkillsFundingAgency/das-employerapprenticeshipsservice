using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IHmrcService
    {
        string GenerateAuthRedirectUrl(string redirectUrl);

        Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
        Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef);
        Task<string> DiscoverEmpref(string authToken);
        Task<LevyDeclarations> GetLevyDeclarations(string empRef);
        Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef);
        Task<DateTime> GetLastEnglishFractionUpdate();
        Task<HmrcTokenResponse> GetOgdAuthenticationToken();
        Task<LevyDeclarations> GetLevyDeclarations(string empRef,DateTime? fromDate);
    }
}