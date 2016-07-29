using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<string> SendMessage<T>(T content, string url);
    }
}
