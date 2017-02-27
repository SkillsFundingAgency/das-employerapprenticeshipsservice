namespace SFA.DAS.EAS.Domain.Http
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException()
            : base(500, "Service is unavailable")
        {
        }
    }
}