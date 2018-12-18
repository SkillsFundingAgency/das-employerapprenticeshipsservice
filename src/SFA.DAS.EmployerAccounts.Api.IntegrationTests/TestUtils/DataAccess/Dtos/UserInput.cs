using System;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class UserInput
    {
        public Guid Ref { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}