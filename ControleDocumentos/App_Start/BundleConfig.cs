using System.Web;
using System.Web.Optimization;

namespace ControleDocumentos
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.11.0.min.js",
                        "~/Scripts/jquery-migrate-1.2.1.min.js",
                        "~/Scripts/jquery.sparkline.min.js",
                        "~/Scripts/jquery.flot.min.js",
                        "~/Scripts/jquery.flot.pie.min.js",
                        "~/Scripts/jquery.flot.stack.js",
                        "~/Scripts/jquery-ui-1.10.3.custom.min.js",
                        "~/Scripts/jquery.flot.time.min.js",
                        //"~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/jquery.flot.resize.min.js",
                        "~/Scripts/jquery.noty.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/util").Include(
                    "~/Scripts/PageScripts/Util.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/retina.js",
                      "~/Scripts/justgage.1.0.1.min.js",
                      "~/Scripts/custom.min.js",
                      "~/Scripts/core.min.js",
                      "~/Scripts/raphael.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/style.css",
                      "~/Content/css/retina.css"
                      //"~/Content/css/jquery.dataTables.min.css"
                      ));

        }
    }
}