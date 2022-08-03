using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public interface IEmployerFinanceApiService
    {
        Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken));
    }
}