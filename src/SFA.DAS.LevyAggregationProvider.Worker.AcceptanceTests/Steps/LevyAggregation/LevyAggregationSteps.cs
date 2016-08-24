using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.LevyAggregationProvider.Worker.AcceptanceTests.Steps.LevyAggregation
{
    [Binding]
    public class LevyAggregationSteps
    {
        [Given(@"I have added ""(.*)"" to my account")]
        public void GivenIHaveAddedToMyAccount(string scenarioFile)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I build the aggregation")]
        public void WhenIBuildTheAggregation()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the result is equal to ""(.*)""")]
        public void ThenTheResultIsEqualTo(string scenarioResultFile)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
