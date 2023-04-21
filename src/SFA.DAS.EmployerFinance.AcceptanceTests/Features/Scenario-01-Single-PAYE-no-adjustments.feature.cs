﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.EmployerFinance.AcceptanceTests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("HMRC-Scenario-01-Single-PAYE-no-adjustments")]
    public partial class HMRC_Scenario_01_Single_PAYE_No_AdjustmentsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Scenario-01-Single-PAYE-no-adjustments.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HMRC-Scenario-01-Single-PAYE-no-adjustments", null, ProgrammingLanguage.CSharp, ((string[])(null)));
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
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Month-01-submission")]
        public virtual void Month_01_Submission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month-01-submission", null, ((string[])(null)));
#line 3
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 4
 testRunner.Given("We have an account with a paye scheme", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate",
                        "CreatedDate"});
            table14.AddRow(new string[] {
                        "999000101",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15",
                        "2017-05-15"});
#line 5
 testRunner.And("Hmrc return the following submissions for paye scheme", ((string)(null)), table14, "And ");
#line 8
 testRunner.When("we refresh levy data for paye scheme on the 06/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 9
 testRunner.And("all the transaction lines in this scenario have had their transaction date update" +
                    "d to their created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
 testRunner.Then("we should see a level 1 screen with a balance of 11000 on the 05/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 11
 testRunner.And("we should see a level 1 screen with a total levy of 11000 on the 05/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And("we should see a level 2 screen with a levy declared of 10000 on the 05/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 13
 testRunner.And("we should see a level 2 screen with a top up of 1000 on the 05/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Month-02-submission")]
        public virtual void Month_02_Submission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month-02-submission", null, ((string[])(null)));
#line 15
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 16
 testRunner.Given("We have an account with a paye scheme", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate"});
            table15.AddRow(new string[] {
                        "999000102",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15"});
            table15.AddRow(new string[] {
                        "999000103",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15"});
            table15.AddRow(new string[] {
                        "999000104",
                        "20000",
                        "17-18",
                        "2",
                        "1",
                        "2017-06-15"});
#line 17
 testRunner.And("Hmrc return the following submissions for paye scheme", ((string)(null)), table15, "And ");
#line 22
 testRunner.When("we refresh levy data for paye scheme on the 08/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 23
 testRunner.And("all the transaction lines in this scenario have had their transaction date update" +
                    "d to their created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 24
 testRunner.Then("we should see a level 1 screen with a balance of 22000 on the 06/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 25
 testRunner.And("we should see a level 1 screen with a total levy of 11000 on the 06/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 26
 testRunner.And("we should see a level 2 screen with a levy declared of 10000 on the 06/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 27
 testRunner.And("we should see a level 2 screen with a top up of 1000 on the 06/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Month-03-submission")]
        public virtual void Month_03_Submission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month-03-submission", null, ((string[])(null)));
#line 29
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 30
 testRunner.Given("We have an account with a paye scheme", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate"});
            table16.AddRow(new string[] {
                        "999000105",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15"});
            table16.AddRow(new string[] {
                        "999000106",
                        "20000",
                        "17-18",
                        "2",
                        "1",
                        "2017-06-15"});
            table16.AddRow(new string[] {
                        "999000107",
                        "30000",
                        "17-18",
                        "3",
                        "1",
                        "2017-07-15"});
#line 31
 testRunner.And("Hmrc return the following submissions for paye scheme", ((string)(null)), table16, "And ");
#line 36
 testRunner.When("we refresh levy data for paye scheme on the 08/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.And("all the transaction lines in this scenario have had their transaction date update" +
                    "d to their created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 38
 testRunner.Then("we should see a level 1 screen with a balance of 33000 on the 07/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 39
 testRunner.And("we should see a level 1 screen with a total levy of 11000 on the 07/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 40
 testRunner.And("we should see a level 2 screen with a levy declared of 10000 on the 07/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 41
 testRunner.And("we should see a level 2 screen with a top up of 1000 on the 07/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Month-06-submission")]
        public virtual void Month_06_Submission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month-06-submission", null, ((string[])(null)));
#line 43
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 44
 testRunner.Given("We have an account with a paye scheme", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table17 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate"});
            table17.AddRow(new string[] {
                        "999000108",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15"});
            table17.AddRow(new string[] {
                        "999000109",
                        "20000",
                        "17-18",
                        "2",
                        "1",
                        "2017-06-15"});
            table17.AddRow(new string[] {
                        "999000110",
                        "30000",
                        "17-18",
                        "3",
                        "1",
                        "2017-07-15"});
            table17.AddRow(new string[] {
                        "999000111",
                        "40000",
                        "17-18",
                        "4",
                        "1",
                        "2017-08-15"});
            table17.AddRow(new string[] {
                        "999000112",
                        "50000",
                        "17-18",
                        "5",
                        "1",
                        "2017-09-15"});
            table17.AddRow(new string[] {
                        "999000113",
                        "60000",
                        "17-18",
                        "6",
                        "1",
                        "2017-10-15"});
#line 45
 testRunner.And("Hmrc return the following submissions for paye scheme", ((string)(null)), table17, "And ");
#line 53
 testRunner.When("we refresh levy data for paye scheme on the 11/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 54
 testRunner.And("all the transaction lines in this scenario have had their transaction date update" +
                    "d to their created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 55
 testRunner.Then("we should see a level 1 screen with a balance of 66000 on the 10/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 56
 testRunner.And("we should see a level 1 screen with a total levy of 11000 on the 10/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 57
 testRunner.And("we should see a level 2 screen with a levy declared of 10000 on the 10/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 58
 testRunner.And("we should see a level 2 screen with a top up of 1000 on the 10/2017", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Month-12-submission")]
        public virtual void Month_12_Submission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Month-12-submission", null, ((string[])(null)));
#line 60
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 61
 testRunner.Given("We have an account with a paye scheme", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table18 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "LevyDueYtd",
                        "Payroll_Year",
                        "Payroll_Month",
                        "English_Fraction",
                        "SubmissionDate"});
            table18.AddRow(new string[] {
                        "999000114",
                        "10000",
                        "17-18",
                        "1",
                        "1",
                        "2017-05-15"});
            table18.AddRow(new string[] {
                        "999000115",
                        "20000",
                        "17-18",
                        "2",
                        "1",
                        "2017-06-15"});
            table18.AddRow(new string[] {
                        "999000116",
                        "30000",
                        "17-18",
                        "3",
                        "1",
                        "2017-07-15"});
            table18.AddRow(new string[] {
                        "999000117",
                        "40000",
                        "17-18",
                        "4",
                        "1",
                        "2017-08-15"});
            table18.AddRow(new string[] {
                        "999000118",
                        "50000",
                        "17-18",
                        "5",
                        "1",
                        "2017-09-15"});
            table18.AddRow(new string[] {
                        "999000119",
                        "60000",
                        "17-18",
                        "6",
                        "1",
                        "2017-10-15"});
            table18.AddRow(new string[] {
                        "999000120",
                        "70000",
                        "17-18",
                        "7",
                        "1",
                        "2017-11-15"});
            table18.AddRow(new string[] {
                        "999000121",
                        "80000",
                        "17-18",
                        "8",
                        "1",
                        "2017-12-15"});
            table18.AddRow(new string[] {
                        "999000122",
                        "90000",
                        "17-18",
                        "9",
                        "1",
                        "2018-01-15"});
            table18.AddRow(new string[] {
                        "999000123",
                        "100000",
                        "17-18",
                        "10",
                        "1",
                        "2018-02-15"});
            table18.AddRow(new string[] {
                        "999000124",
                        "110000",
                        "17-18",
                        "11",
                        "1",
                        "2018-03-15"});
            table18.AddRow(new string[] {
                        "999000125",
                        "120000",
                        "17-18",
                        "12",
                        "1",
                        "2018-04-15"});
#line 62
 testRunner.And("Hmrc return the following submissions for paye scheme", ((string)(null)), table18, "And ");
#line 76
 testRunner.When("we refresh levy data for paye scheme on the 05/2018", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 77
 testRunner.And("all the transaction lines in this scenario have had their transaction date update" +
                    "d to their created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 78
 testRunner.Then("we should see a level 1 screen with a balance of 132000 on the 04/2018", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 79
 testRunner.And("we should see a level 1 screen with a total levy of 11000 on the 04/2018", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 80
 testRunner.And("we should see a level 2 screen with a levy declared of 10000 on the 04/2018", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 81
 testRunner.And("we should see a level 2 screen with a top up of 1000 on the 04/2018", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion