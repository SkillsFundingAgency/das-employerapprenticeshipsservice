using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services;

public interface IPayeLevySubmissionsHandler
{
    Task<PayeLevySubmissionsResponse> FindPayeSchemeLevySubmissions(string accountId, string payeId);
}