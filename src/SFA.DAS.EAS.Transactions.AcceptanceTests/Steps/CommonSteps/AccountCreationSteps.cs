using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class AccountCreationSteps
    {
        [Given(@"I have an account")]
        public void GivenIHaveAnAccount()
        {
            ScenarioContext.Current.Pending();
        }

    }
}
