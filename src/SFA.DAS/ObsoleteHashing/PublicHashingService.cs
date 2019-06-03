namespace SFA.DAS.ObsoleteHashing
{
    public class PublicHashingService : HashingService.HashingService, IPublicHashingService, IAccountLegalEntityPublicHashingService
    {
        public PublicHashingService(string allowedCharacters, string hashstring)
            : base(allowedCharacters, hashstring)
        {
        }
    }
}