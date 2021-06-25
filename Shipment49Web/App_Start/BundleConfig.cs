using System.Web;
using System.Web.Optimization;

namespace Shipment49Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                         "~/Scripts/dropzone.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //           "~/Scripts/jquery-1.10.2.min.js",
            //           "~/Scripts/jquery-2.1.3.min.js",
            //           "~/Scripts/jquery.validate.min.js",
            //           "~/Scripts/jquery.min.js",
            //           "~/Scripts/jquery.FlowupLabels.js",
            //           "~/Scripts/jquery.validate.unobtrusive.min.js",
            //           "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate.min.js",
                       "~/Scripts/jquery.FlowupLabels.js",
                       "~/Scripts/jquery.validate.unobtrusive.min.js",
                       "~/Scripts/main.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryvaldropdown").Include(
            //          "~/ScriptBMS/angular.js",
            //          "~/ScriptBMS/lodash.js",
            //            "~/ScriptBMS/angularjs-dropdown-multiselect.js",
            //            "~/Scripts/MyApp.js"));


            //bundles.Add(new ScriptBundle("~/bundles/multiselectjs").Include(
            //           "~/MultiselectDropdown/bootstrap-multiselect.js"));

            //bundles.Add(new StyleBundle("~/bundles/multiselectcss").Include(
            //             "~/MultiselectDropdown/bootstrap-multiselect.css"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/themes/jquery-ui.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/cssform").Include(
                     "~/Content/jquery.FlowupLabels.css"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                        "~/Scripts/dropzone/dropzone.min.js"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                     "~/Scripts/dropzone/css/basic.css",
                     "~/Scripts/dropzone/css/dropzone.css"));

            //bundles.Add(new ScriptBundle("~/bundles/dropzones").Include(
            //      "~/Scripts/dropzone/dropzone.js"));

            //bundles.Add(new StyleBundle("~/Content/dropzones-css").Include(
            //         "~/Scripts/dropzone/css/basic.css",
            //         "~/Scripts/dropzone/css/dropzone.css"));

        }
    }
}
