using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Steps
{
    [Binding]
    public class FakeLandingPageSteps
    {
        [Given(@"I have Navigated to Landing Page")]
        public void GivenIHaveNavigatedToLandingPage()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I will see ""(.*)""")]
        public void ThenIWillSee(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
