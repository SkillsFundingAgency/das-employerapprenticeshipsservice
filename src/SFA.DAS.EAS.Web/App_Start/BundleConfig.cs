//using Karambolo.AspNetCore.Bundling.NUglify;
//using Microsoft.AspNetCore.Builder;

//namespace SFA.DAS.EAS.Web
//{
//    public class BundleConfig
//    {
//        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
//        public static void RegisterBundles(BundleCollectionConfigurer bundles)
//        {
//            bundles.AddJs("~/bundles/sfajs")
//                    .Include("~/dist/javascripts/jquery-1.11.0.min.js")
//                    .Include("~/dist/javascripts/govuk-template.js")
//                    .Include("~/dist/javascripts/selection-buttons.js")
//                    .Include("~/dist/javascripts/showhide-content.js")
//                    .Include("~/dist/javascripts/stacker.js")
//                    .Include("~/dist/javascripts/app.js");

//            bundles.AddJs("~/bundles/apprentice")
//                    .Include("~/dist/javascripts/apprentice/select2.min.js")
//                    .Include("~/dist/javascripts/apprentice/dropdown.js");

//            bundles.AddJs("~/bundles/characterLimitation")
//                    .Include("~/dist/javascripts/character-limit.js");

//            bundles.AddJs("~/bundles/lengthLimitation")
//                    .Include("~/dist/javascripts/length-limit.js");

//            bundles.AddJs("~/bundles/paymentOrder")
//                    .Include("~/dist/javascripts/payment-order.js");

//            bundles.AddJs("~/bundles/jqueryvalcustom")
//                    .Include("~/Scripts/jquery.validate.js")
//                    .Include("~/Scripts/jquery.validate.unobtrusive.custom.js");

//            bundles.AddJs("~/bundles/modernizr")
//                    .Include("~/dist/javascripts/sfa-modernizr.js");

//            // This is a temporary fix while the asset references in the EmployerCommitments site are sorted out. 05/10/2017
//            bundles.AddJs("~/bundles/lodash")
//                    .Include("~/dist/javascripts/lodash.js");

//            bundles.AddCss("~/bundles/screenie6").Include("~/dist/css/screen-ie6.css");
//            bundles.AddCss("~/bundles/screenie7").Include("~/dist/css/screen-ie7.css");
//            bundles.AddCss("~/bundles/screenie8").Include("~/dist/css/screen-ie8.css");
//            bundles.AddCss("~/bundles/screen").Include("~/dist/css/screen.css");

//        }
//    }
//}
