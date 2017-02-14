namespace SFA.DAS.EAS.Infrastructure.Interfaces.REST
{
    public interface IRestServiceFactory
    {
        IRestService Create(string baseUrl);
    }
}