using FeatureToggle;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public class FeatureToggleViewEngine : RazorViewEngine
    {
        private readonly IBooleanToggleValueProvider _booleanToggleValueProvider;
        private readonly bool _isHomePageFeature;
        public FeatureToggleViewEngine(IBooleanToggleValueProvider booleanToggleValueProvider)
            : base()
        {            
            PartialViewLocationFormats = PartialViewLocationFormats.Concat(new[] { "~/Views/{1}/V2/{0}.cshtml" }).ToArray();

            _booleanToggleValueProvider = booleanToggleValueProvider;
            _isHomePageFeature = (new HomePageFeature(_booleanToggleValueProvider)).FeatureEnabled;
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (_isHomePageFeature && viewPath.Contains("~/Views/EmployerTeam/"))
            {
                return base.CreateView(controllerContext, viewPath.Replace("~/Views/EmployerTeam/", "~/Views/EmployerTeam/V2/"), "~/Views/Shared/_Layout_v2.cshtml");
            }

            return base.CreateView(controllerContext, viewPath, masterPath);
        }
    }
}