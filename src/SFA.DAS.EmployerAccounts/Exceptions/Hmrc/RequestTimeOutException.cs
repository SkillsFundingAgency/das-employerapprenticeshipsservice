namespace SFA.DAS.EmployerAccounts.Exceptions.Hmrc
{
    public class RequestTimeOutException : HttpException
    {
        public RequestTimeOutException() : base(408, "Request has time out")
        {
        }
    }
}