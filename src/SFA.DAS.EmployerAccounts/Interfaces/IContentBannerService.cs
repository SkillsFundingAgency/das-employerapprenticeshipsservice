using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IContentBannerService
    {
        Task<string> GetBannerContent(int bannerId, bool useCDN);
    }
}
