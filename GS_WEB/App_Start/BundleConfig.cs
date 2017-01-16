using System.Web.Optimization;

namespace GS_WEB
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/jquery-2.1.4.js",
                        "~/Scripts/jquery-ui-1.11.4.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/common.js"));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/validation.css"));
                      //"~/Content/themes/base/*.css"

            bundles.Add(new StyleBundle("~/Content/themes/base/jqui_css_bundle").Include(
                      "~/Content/themes/base/*.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
