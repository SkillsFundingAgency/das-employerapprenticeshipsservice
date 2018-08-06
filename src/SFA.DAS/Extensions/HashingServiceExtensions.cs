using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Extensions
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
