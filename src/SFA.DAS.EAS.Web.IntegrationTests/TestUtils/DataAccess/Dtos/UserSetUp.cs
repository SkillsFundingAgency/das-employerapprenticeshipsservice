using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class UserSetup
    {
        public UserInput UserInput { get; set; }
        public UserOutput UserOutput { get; set; }
        public List<EmployerAccountSetup> Accounts { get; } = new List<EmployerAccountSetup>();
    }
}