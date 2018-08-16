using SFA.DAS.EAS.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.Application.Hashing
{
    public class PublicHashingService : HashingService.HashingService, IPublicHashingService, IALEPublicHashingService
    {
        public PublicHashingService(string allowedCharacters, string hashstring) : base(allowedCharacters, hashstring)
        {
        }
    }
}