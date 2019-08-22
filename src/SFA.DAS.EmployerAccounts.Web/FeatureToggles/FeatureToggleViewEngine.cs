using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public class NewHomepageViewEngine : RazorViewEngine
    {
        public NewHomepageViewEngine()
            : base()
        {            
            PartialViewLocationFormats = PartialViewLocationFormats.Concat(new[] { "~/Views/{1}/V2/{0}.cshtml" }).ToArray();
        }
    }
}