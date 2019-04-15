using System;
using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories
{
    public class TestCurrentDateTime : ICurrentDateTime
    {
        public TestCurrentDateTime()
        {
            Now = DateTime.UtcNow;
        }

        public DateTime Now { get; set; }
    }
}
