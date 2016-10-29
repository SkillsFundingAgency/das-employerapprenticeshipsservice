using System.Web.Optimization;

namespace SFA.DAS.EAS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalcustom").Include(
                        "~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/footer").Include(
                      "~/assets/javascripts/jquery-1.11.0.min.js",
                      "~/assets/javascripts/govuk-template.js",
                      "~/assets/javascripts/app.js"));

               bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/elements.css",
                      "~/Content/site.css"));
        }
    }
}
