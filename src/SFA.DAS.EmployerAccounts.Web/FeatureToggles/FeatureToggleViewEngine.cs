using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public class FeatureToggleViewEngine : RazorViewEngine
    {
        public FeatureToggleViewEngine()
            : base()
        {            
            PartialViewLocationFormats = PartialViewLocationFormats.Concat(new[] { "~/Views/{1}/V2/{0}.cshtml" }).ToArray();
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (Features.HomePage.Enabled && viewPath.Contains("~/Views/EmployerTeam/"))
            {
                return base.CreateView(controllerContext, viewPath.Replace("~/Views/EmployerTeam/", "~/Views/EmployerTeam/V2/"), "~/Views/Shared/_Layout_v2.cshtml");
            }

            return base.CreateView(controllerContext, viewPath, masterPath);
        }
    }
}