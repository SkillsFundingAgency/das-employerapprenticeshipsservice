namespace SFA.DAS.EmployerAccounts.Exceptions.Hmrc
{
    public class InternalServerErrorException : HttpException
    {
        public InternalServerErrorException()
            : base(500, "Internal server error")
        {
        }
    }
}