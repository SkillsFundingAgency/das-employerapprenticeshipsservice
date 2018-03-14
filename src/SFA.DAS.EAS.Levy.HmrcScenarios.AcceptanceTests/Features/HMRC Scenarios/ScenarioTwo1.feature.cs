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
    [NUnit.Framework.DescriptionAttribute("Scenario Two - Seasonal variations, single PAYE")]
    public partial class ScenarioTwo_SeasonalVariationsSinglePAYEFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "ScenarioTwo.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Scenario Two - Seasonal variations, single PAYE", null, ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Month three submission")]
        public virtual void MonthThreeSubmission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month three submission", ((string[])(null)));
#line 3
this.ScenarioSetup(scenarioInfo);
#line 4
 testRunner.Given("I have an account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Paye_scheme",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate",
                        "CreatedDate"});
            table1.AddRow(new string[] {
                        "123/ABC",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15",
                        "2017-05-23"});
            table1.AddRow(new string[] {
                        "123/ABC",
                        "20000",
                        "17-18",
                        "2",
                        "1",
                        "2017-06-15",
                        "2017-06-23"});
            table1.AddRow(new string[] {
                        "123/ABC",
                        "18750",
                        "17-18",
                        "3",
                        "1",
                        "2017-07-15",
                        "2017-07-23"});
#line 5
 testRunner.When("I have the following submissions", ((string)(null)), table1, "When ");
#line 10
 testRunner.Then("the balance on 07/2017 should be 20625 on the screen", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 11
 testRunner.And("the total levy shown for month 07/2017 should be -1375", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And("For month 07/2017 the levy declared should be -1250 and the topup should be -125", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Month twelve submission (Checking 2nd negative declaration)")]
        public virtual void MonthTwelveSubmissionChecking2NdNegativeDeclaration()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month twelve submission (Checking 2nd negative declaration)", ((string[])(null)));
#line 14
this.ScenarioSetup(scenarioInfo);
#line 15
 testRunner.Given("I have an account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Paye_scheme",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate",
                        "CreatedDate"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15",
                        "2017-05-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "20000",
                        "17-18",
                        "2",
                        "1",
                        "2017-06-15",
                        "2017-06-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "18750",
                        "17-18",
                        "3",
                        "1",
                        "2017-07-15",
                        "2017-07-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "28750",
                        "17-18",
                        "4",
                        "1",
                        "2017-08-15",
                        "2017-08-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "38750",
                        "17-18",
                        "5",
                        "1",
                        "2017-09-15",
                        "2017-09-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "48750",
                        "17-18",
                        "6",
                        "1",
                        "2017-10-15",
                        "2017-10-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "58750",
                        "17-18",
                        "7",
                        "1",
                        "2017-11-15",
                        "2017-11-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "68750",
                        "17-18",
                        "8",
                        "1",
                        "2017-12-15",
                        "2017-12-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "67500",
                        "17-18",
                        "9",
                        "1",
                        "2018-01-15",
                        "2018-01-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "77500",
                        "17-18",
                        "10",
                        "1",
                        "2018-02-15",
                        "2018-02-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "87500",
                        "17-18",
                        "11",
                        "1",
                        "2018-03-15",
                        "2018-03-23"});
            table2.AddRow(new string[] {
                        "123/ABC",
                        "97500",
                        "17-18",
                        "12",
                        "1",
                        "2018-04-15",
                        "2018-04-23"});
#line 16
 testRunner.When("I have the following submissions", ((string)(null)), table2, "When ");
#line 30
 testRunner.Then("the balance on 04/2018 should be 107250 on the screen", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 31
 testRunner.And("the total levy shown for month 01/2018 should be -1375", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 32
 testRunner.And("For month 01/2018 the levy declared should be -1250 and the topup should be -125", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
