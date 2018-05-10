using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IHmrcService
    {
        string GenerateAuthRedirectUrl(string redirectUrl);

        Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
        Task<HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef);
        Task<string> DiscoverEmpref(string authToken);
        Task<HMRC.ESFA.Levy.Api.Types.LevyDeclarations> GetLevyDeclarations(string empRef);
        Task<HMRC.ESFA.Levy.Api.Types.EnglishFractionDeclarations> GetEnglishFractions(string empRef);
        Task<DateTime> GetLastEnglishFractionUpdate();
        Task<string> GetOgdAccessToken();
        Task<HMRC.ESFA.Levy.Api.Types.LevyDeclarations> GetLevyDeclarations(string empRef,DateTime? fromDate);
        Task<HMRC.ESFA.Levy.Api.Types.EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate);
        Task<HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation> GetEmprefInformation(string empRef);
    }
}