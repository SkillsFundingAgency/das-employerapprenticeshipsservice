﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.2.0.0
//      SpecFlow Generator Version:2.2.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.EAS.Levy.HmrcScenarios.AcceptanceTests2.Features.HMRCScenarios
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.2.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Scenario Seven - \"No Payment for Period\" & Ceased PAYE Scheme")]
    public partial class ScenarioSeven_NoPaymentForPeriodCeasedPAYESchemeFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "ScenarioSeven.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Scenario Seven - \"No Payment for Period\" & Ceased PAYE Scheme", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Balance should remain if no payment occurs")]
        public virtual void BalanceShouldRemainIfNoPaymentOccurs()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Balance should remain if no payment occurs", ((string[])(null)));
#line 3
this.ScenarioSetup(scenarioInfo);
#line 4
 testRunner.Given("I have an account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 5
 testRunner.And("I add a PAYE Scheme 123/ABC with name Test Corp to the account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "SubmissionId",
                        "PAYEScheme",
                        "LevyDueYTD",
                        "PayrollPeriod",
                        "PayrollMonth",
                        "SubmissionTime",
                        "CreatedDate",
                        "LevyAllowanceForFullYear",
                        "NoPaymentForPeriod",
                        "DateCeased"});
            table1.AddRow(new string[] {
                        "1",
                        "1",
                        "123/ABC",
                        "1000",
                        "17-18",
                        "1",
                        "2017-04-15",
                        "2017-04-23",
                        "15000",
                        "",
                        ""});
            table1.AddRow(new string[] {
                        "2",
                        "2",
                        "123/ABC",
                        "2000",
                        "17-18",
                        "2",
                        "2017-05-15",
                        "2017-05-23",
                        "15000",
                        "true",
                        ""});
#line 6
 testRunner.When("I get the following declarations from HMRC", ((string)(null)), table1, "When ");
#line 10
 testRunner.Then("the balance on 06/2017 should be 1100 on the screen", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Balance should not be affected by past non payment months")]
        public virtual void BalanceShouldNotBeAffectedByPastNonPaymentMonths()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Balance should not be affected by past non payment months", ((string[])(null)));
#line 13
this.ScenarioSetup(scenarioInfo);
#line 14
 testRunner.Given("I have an account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 15
 testRunner.And("I add a PAYE Scheme 123/ABC with name Test Corp to the account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "SubmissionId",
                        "PAYEScheme",
                        "LevyDueYTD",
                        "PayrollPeriod",
                        "PayrollMonth",
                        "SubmissionTime",
                        "CreatedDate",
                        "LevyAllowanceForFullYear",
                        "NoPaymentForPeriod",
                        "DateCeased"});
            table2.AddRow(new string[] {
                        "1",
                        "1",
                        "123/ABC",
                        "1000",
                        "17-18",
                        "1",
                        "2017-04-15",
                        "2017-04-23",
                        "15000",
                        "",
                        ""});
            table2.AddRow(new string[] {
                        "2",
                        "2",
                        "123/ABC",
                        "0",
                        "17-18",
                        "2",
                        "2017-05-15",
                        "2017-05-23",
                        "15000",
                        "true",
                        ""});
            table2.AddRow(new string[] {
                        "3",
                        "3",
                        "123/ABC",
                        "2000",
                        "17-18",
                        "3",
                        "2017-06-15",
                        "2017-06-23",
                        "15000",
                        "",
                        ""});
#line 16
 testRunner.When("I get the following declarations from HMRC", ((string)(null)), table2, "When ");
#line 21
 testRunner.Then("the balance on 06/2017 should be 2200 on the screen", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
