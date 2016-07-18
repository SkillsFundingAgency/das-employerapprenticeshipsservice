using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Setup
{
    public abstract class CoreSettings
    {
        public  IWebDriver Browser ;
        //public static IWebDriver Browser = new ChromeDriver();

        public static string BaseUrl = ConfigurationManager.AppSettings.Get("BaseUrl");

        #region Navigation
        public  void NavigatetoBasePage()
        {
            try
            {
                Browser.Navigate().GoToUrl(BaseUrl);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void NavigateSpecificUrl(string Url)
        {
            try
            {
                Browser.Navigate().GoToUrl(Url);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Closing Browser
        public  void CloseBrowser()
        {
            try
            {
                Browser.Quit();
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region Finding Elements
        public  void FindElement(string element, string selector)
        {
            try
            {
                if (selector == "Name")
                {
                    Browser.FindElement(By.Name(element));
                }

                else if (selector == "Id")
                {
                    Browser.FindElement(By.Id(element));
                }

                else if (selector == "Css")
                {
                    Browser.FindElement(By.CssSelector(element));
                }

                else if (selector == "Xpath")
                {
                    Browser.FindElement(By.XPath(element));
                }
                
            }
            catch (Exception)
            {

                throw;
            }

        }
        
        #endregion

        #region Clicking Elements
        public  void ClickElement(string element, string selector)
        {
            try
            {
                if (selector == "Name")
                {
                    WebDriverWait wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(10));
                    var elementNowVisible = wait.Until(ExpectedConditions.ElementIsVisible(By.Name(element)));
                    Actions action = new Actions(Browser);
                    action.MoveToElement(elementNowVisible).Click();
                }

                else if (selector == "Id")
                {
                    Browser.FindElement(By.Id(element)).Click();
                }

                else if (selector == "Css")
                {
                    Browser.FindElement(By.CssSelector(element)).Click();
                }

                else if (selector == "Xpath")
                {
                    Browser.FindElement(By.XPath(element)).Click();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        public void MouseOverElementandClick(string element, string selector)
        {
           try
            {
                if (selector == "Name")
                {
                    var webElement = Browser.FindElement(By.Name(element));
                    Actions action = new Actions(Browser);
                    action.MoveToElement(webElement).Click();

                }

                else if (selector == "Id")
                {
                    var webElement = Browser.FindElement(By.Id(element));
                    Actions action = new Actions(Browser);
                    action.MoveToElement(webElement).Click();
                }

                else if (selector == "Css")
                {
                    var webElement = Browser.FindElement(By.CssSelector(element));
                    Actions action = new Actions(Browser);
                    action.MoveToElement(webElement).Click();
                }

                else if (selector == "Xpath")
                {
                    var webElement = Browser.FindElement(By.XPath(element));
                    Actions action = new Actions(Browser);
                    action.MoveToElement(webElement).Click();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Check Current Page Title
        public void CheckCurrentPageTitle(string Title)
        {
            try
            {
                string actualPageTitle = Browser.Title;
                Assert.AreEqual(actualPageTitle, Title);
                    
                
            }
            catch (Exception)
            {

                throw;
            }


        }
        #endregion
        #region Reading  Page Content
        public void CheckTextOnPageBy(string element, string SelectorType, string ExpectedText)
        {
            try
            {
                if (SelectorType == "Id")
                {
                    string actualTextById = Browser.FindElement(By.Id(element)).Text;
                    Assert.AreEqual(actualTextById, ExpectedText);
                }
                else if (SelectorType == "Name")
                {
                    string actualTextByName = Browser.FindElement(By.Name(element)).Text;
                    Assert.AreEqual(actualTextByName, ExpectedText);
                }
                else if (SelectorType == "Css")
                {
                    string actualTextByCss = Browser.FindElement(By.CssSelector(element)).Text;
                    Assert.AreEqual(actualTextByCss, ExpectedText);
                }
            }
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Check Text Threw an Exception:", ex);
                throw argEx;
            }           
                           
            
            
        }

        #endregion
        #region Waiting
        public void WaitForUntil(int Time,string element, string selector)
        {
            try
            {
                if (selector=="Name")
                {
                    WebDriverWait wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(Time));
                    var elementNowVisible = wait.Until(ExpectedConditions.ElementIsVisible(By.Name(element)));
                }

                else if (selector == "Id")
                {
                    WebDriverWait wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(Time));
                    var elementNowVisible = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(element)));
                }

                else if (selector == "Css")
                {
                    WebDriverWait wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(Time));
                    var elementNowVisible = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(element)));
                }

                else if (selector == "Xpath")
                {
                    WebDriverWait wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(Time));
                    var elementNowVisible = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(element)));
                }

            }
            catch (Exception)
            {

                throw;
            }

            
        }
        #endregion

    }
}
