using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<string> SendMessage<T>(T content, string url);
        Task<T> Get<T>(string authToken, string url);
    }
}
