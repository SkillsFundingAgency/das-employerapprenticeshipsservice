using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IContentBannerApiClient
    {
        Task<string> GetBanner(int bannerId, bool useCDN);
    }
}
