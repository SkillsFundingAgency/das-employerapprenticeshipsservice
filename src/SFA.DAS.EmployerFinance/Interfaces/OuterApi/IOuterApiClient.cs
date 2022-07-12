using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces.OuterApi
{
    public interface IOuterApiClient
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
    }
}