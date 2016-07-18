using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Shared
{
    
    public class BasePage : CoreSettings
    {
        public BasePage(string BrowserType)
        {
            switch(BrowserType)
            {
                case "Phantom":
                    Browser = new PhantomJSDriver();
                    break;
                case "Chrome":
                    Browser = new ChromeDriver();
                    break;
            }

        }

        public BasePage()
        {

        }

       

    }
}
