using System.Web.Optimization;

namespace SFA.DAS.EAS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/dist/js/sfa.min.js").Include(
                      "~/dist/javascripts/jquery-1.11.0.min.js",
                      "~/dist/javascripts/govuk-template.js",
                      "~/dist/javascripts/selection-buttons.js",
                      "~/dist/javascripts/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalcustom").Include(
                      "~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.custom.js"));
        }
    }
}
