namespace SFA.DAS.EAS.Infrastructure.Http
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException()
            : base(503, "Service is unavailable")
        {
        }
    }
}