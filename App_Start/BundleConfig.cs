using System.Web;
using System.Web.Optimization;

namespace EMPLOYEE_MANAGEMENT
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new Bundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            BundleTable.EnableOptimizations = false;
            string ltecomponents = "~/Content/";

            bundles.Add(new StyleBundle("~/Content/css")
             .Include(ltecomponents + "resources/fa/css/all.min.css")
             .Include(ltecomponents + "resources/dist/css/staticcss.css")
             .Include(ltecomponents + "resources/dist/css/custom.css")
             .Include(ltecomponents + "resources/select2/select2.css")
             .Include(ltecomponents + "resources/fonts/source-sans-pro.css")
             .Include(ltecomponents + "resources/datepicker/datepicker.css")
             .Include(ltecomponents + "resources/datatables/bs4/dataTables.bootstrap4.css")

        
             );

            bundles.Add(new ScriptBundle("~/bundles/jquery")
            .Include(ltecomponents + "resources/jquery/jquery.js")
            .Include(ltecomponents + "resources/popper/popper.js")
            .Include(ltecomponents + "resources/bootstrap/js/bootstrap.js")
            .Include(ltecomponents + "resources/select2/select2.js")
            .Include(ltecomponents + "resources/dist/js/adminlte.js")
            .Include(ltecomponents + "resources/datepicker/datepicker.js")
            .Include(ltecomponents + "resources/datatables/jquery.dataTables.js")

            
            );
        }
    }
}
