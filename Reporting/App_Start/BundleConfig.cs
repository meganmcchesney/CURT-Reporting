using System.Web;
using System.Web.Optimization;

namespace Reporting {
    public class BundleConfig {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {
            /*bundles.Add(new ScriptBundle("~/bundles/boilerplate").Include(
                "~/Scripts/vendor/jquery-1.8.2.min.js",
                "~/Scripts/vendor/jquery-ui-1.9.1.custom.min.js",
                "~/Scripts/vendor/bootstrap.min.js",
                "~/Scripts/vendor/plugins.js"));*/
            bundles.Add(new ScriptBundle("~/bundles/boilerplate").IncludeDirectory("~/Scripts/vendor/","*.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-2.6.1*"));

            bundles.Add(new StyleBundle("~/Content/main-css").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-responsive.css",
                "~/Content/css/smoothness/jquery-ui.css",
                "~/Content/css/normalize.css",
                "~/Content/css/main.css",
                "~/Content/css/MeganStyle.css"));
        }
    }
}