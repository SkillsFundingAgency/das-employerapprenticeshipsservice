namespace SFA.DAS.EmployerAccounts.Exceptions.Hmrc
{
    public class ResourceNotFoundException : HttpException
    {
        public ResourceNotFoundException(string resourceUri)
            : base(404, "Could not find requested resource - " + resourceUri)
        {
        }
    }
}