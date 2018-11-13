using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.TestCommon.TestModels
{
    public class TestUser
    {
        public long Id { get; set; }
        public string UserRef { get; set; }
        public IEnumerable<TestAccount> Accounts { get; set; }
    }
}
