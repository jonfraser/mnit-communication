using System.Web;
using System.Web.Optimization;

namespace MNIT_Communication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/ui-bootstrap")
                .Include("~/Scripts/ui-bootstrap-tpls-*"));

            bundles.Add(new ScriptBundle("~/bundles/moment")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/angular-moment.js")
                .Include("~/Scripts/moment.setLocale.en-AU.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap/bootstrap.css",
                         "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapSwitch")   
                    .Include("~/Scripts/bootstrap-switch.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrapSwitch")
                    .Include("~/Content/bootstrap-switch.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.datetimepicker")
                    .Include("~/Scripts/jquery.datetimepicker.js"));

            bundles.Add(new StyleBundle("~/Content/jquery.datetimepicker")
                    .Include("~/Content/jquery.datetimepicker.css"));
        }
    }
}
