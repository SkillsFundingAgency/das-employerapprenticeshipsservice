using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Contracts.OuterApi;

public interface IOuterApiClient
{
    Task<TResponse> Get<TResponse>(IGetApiRequest request);
}