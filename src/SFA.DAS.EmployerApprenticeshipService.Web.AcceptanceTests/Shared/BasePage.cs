using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Shared
{
    
    public abstract class BasePage
    {
        public static IWebDriver _driver = CoreSettings.Browser;

        #region Navigation
        public static void Navigate(string Url)
        {
            try
            {
                _driver.Navigate().GoToUrl(Url);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        #endregion

        #region Close Browser
        public static void CloseBrowser()
        {
            try
            {
                _driver.Quit();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        #endregion

        #region Find Elements
        public static void FindElementByName(string element)
        {
            try
            {
                _driver.FindElement(By.Name(element));
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public static void FindElementById(string element)
        {
            try
            {
                _driver.FindElement(By.Id(element));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Click Elements
        public static void ClickElementByName(string element)
        {
            try
            {
                _driver.FindElement(By.Name(element));
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
                Assert.AreEqual(_driver.Title, Title);
            }
            catch (Exception)
            {

                throw;
            }
                                 

        }
        #endregion

    }
}
