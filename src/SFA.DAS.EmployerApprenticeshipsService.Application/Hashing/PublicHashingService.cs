namespace SFA.DAS.EAS.Application.Hashing
{
    public class PublicHashingService : HashingService.HashingService, IPublicHashingService
    {
        public PublicHashingService(string allowedCharacters, string hashstring) 
            : base(allowedCharacters, hashstring)
        {
        }
    }
}