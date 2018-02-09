namespace SFA.DAS.EAS.Application.Hashing
{
    public class ExternalAccountHashingService : HashingService.HashingService, IExternalAccountHashingService
    {
        public ExternalAccountHashingService(string allowedCharacters, string hashstring) : base(allowedCharacters, hashstring)
        {
        }
    }
}