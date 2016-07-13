using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OpenQA.Selenium.Chrome;

namespace SFA.DAS.EmployerApprenticeshipService.Web.AcceptanceTests.Setup
{
    public class CoreSettings
    {
        public static IWebDriver Browser = new ChromeDriver();
        public static string BaseUrl = ConfigurationManager.AppSettings.Get("BaseUrl");
    }
}
