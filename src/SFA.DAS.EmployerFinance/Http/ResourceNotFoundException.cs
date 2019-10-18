namespace SFA.DAS.EmployerFinance.Http
{
    public class ResourceNotFoundException : HttpException
    {
        public ResourceNotFoundException(string resourceUri)
            : base(404, "Could not find requested resource - " + resourceUri)
        {
        }
    }
}