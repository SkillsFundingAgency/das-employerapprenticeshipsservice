namespace SFA.DAS.EAS.Infrastructure.Hashing
{
    public class PublicHashingService : HashingService.HashingService, IPublicHashingService, IALEPublicHashingService
    {
        public PublicHashingService(string allowedCharacters, string hashstring) : base(allowedCharacters, hashstring)
        {
        }
    }
}