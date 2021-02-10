using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces.OuterApi
{
    public interface IApiClient
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
    }
}