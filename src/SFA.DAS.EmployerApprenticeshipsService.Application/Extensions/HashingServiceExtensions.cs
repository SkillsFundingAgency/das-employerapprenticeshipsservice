using System;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Extensions
{
    public static class HashingServiceExtensions
    {
        public static bool TryDecodeValue(this IHashingService hashingService, string input, out long result)
        {
            try
            {
                result = hashingService.DecodeValue(input);
                return true;
            }
            catch (Exception)
            {
                result = default(long);
                return false;
            }
        }
    }
}