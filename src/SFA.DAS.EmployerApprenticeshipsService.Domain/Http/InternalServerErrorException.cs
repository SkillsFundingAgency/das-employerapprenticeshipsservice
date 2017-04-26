namespace SFA.DAS.EAS.Domain.Http
{
    public class InternalServerErrorException : HttpException
    {
        public InternalServerErrorException()
            : base(500, "Internal server error")
        {
        }
    }
}