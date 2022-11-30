using System;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.TestCommon.Helpers
{
    public class TestHashingService : IHashingService
    {
        public long DecodeValue(string id)
        {
            return long.Parse(id);
        }

        public Guid DecodeValueToGuid(string id)
        {
            throw new NotImplementedException();
        }

        public string DecodeValueToString(string id)
        {
            throw new NotImplementedException();
        }

        public string HashValue(long id)
        {
            return id.ToString();
        }

        public string HashValue(Guid id)
        {
            throw new NotImplementedException();
        }

        public string HashValue(string id)
        {
            throw new NotImplementedException();
        }

        public bool TryDecodeValue(string input, out long output)
        {
            throw new NotImplementedException();
        }
    }
}
