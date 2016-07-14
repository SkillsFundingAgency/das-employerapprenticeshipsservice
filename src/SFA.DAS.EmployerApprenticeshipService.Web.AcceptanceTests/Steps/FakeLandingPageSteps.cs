using SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Pages;
using SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Shared;
using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Steps
{
    [Binding]
    public class FakeLandingPageSteps
    {
        BasePage TestPage = new BasePage();
        _2ndPage Test_2ndPage = new _2ndPage();

        [Given(@"I have Navigated to Landing Page")]
        [When(@"I have Navigated to Landing Page")]
        [Then(@"I have Navigated to Landing Page")]
        public void GivenIHaveNavigatedToLandingPage()
        {
            TestPage.NavigatetoBasePage();
            
        }

        [Given(@"I Navigated to URL ""(.*)""")]
        [When(@"I Navigated to URL ""(.*)""")]
        [Then(@"I Navigated to URL ""(.*)""")]
        public void GivenINavigatedToURL(string Url)
        {
            TestPage.NavigateSpecificUrl(Url);
        }

        [Given(@"I Navigated to 2nd Page")]
        [When(@"I Navigated to 2nd Page")]
        [Then(@"I Navigated to 2nd Page")]
        public void ThenINavigatedTo()
        {
            Test_2ndPage.NavigateSpecificUrl(Test_2ndPage.Url);
        }



        [Given(@"I Check for ""(.*)"" found by ""(.*)"" it will read ""(.*)""")]
        [When(@"I Check for ""(.*)"" found by ""(.*)"" it will read ""(.*)""")]
        [Then(@"I Check for ""(.*)"" found by ""(.*)"" it will read ""(.*)""")]
        public void WhenICheckForFoundByItWillRead(string element, string SelectorType, string ExpectedText)
        {
            TestPage.CheckTextOnPageBy(element, SelectorType, ExpectedText);
        }

        [Given(@"I Check the Title of Page it will read ""(.*)""")]
        [When(@"I Check the Title of Page it will read ""(.*)""")]
        [Then(@"I Check the Title of Page it will read ""(.*)""")]
        public void WhenICheckTheTitleOfPageItWillRead(string Title)
        {
            TestPage.CheckCurrentPageTitle(Title);
        }

        [Given(@"I click ""(.*)"" found by ""(.*)""")]
        [When(@"I click ""(.*)"" found by ""(.*)""")]
        [Then(@"I click ""(.*)"" found by ""(.*)""")]
        public void WhenIClickFoundBy(string element, string selector)
        {
            TestPage.ClickElement(element, selector);
        }

        [Given(@"I Mouseover and Click ""(.*)"" found by ""(.*)""")]
        [When(@"I Mouseover and Click ""(.*)"" found by ""(.*)""")]
        [Then(@"I Mouseover and Click ""(.*)"" found by ""(.*)""")]
        public void WhenIMouseoverAndClickFoundBy(string element, string selector)
        {
            TestPage.MouseOverElementandClick(element, selector);
        }

        [Given(@"Browser can be closed")]
        [When(@"Browser can be closed")]
        [Then(@"Browser can be closed")]
        public void ThenBrowserCanBeClosed()
        {
            TestPage.CloseBrowser();
        }





    }
}
