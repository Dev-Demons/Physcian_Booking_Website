using System.Web.Optimization;

namespace Binke
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/system/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/system/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/system/modernizr-*"));

            #region Client Section
            bundles.Add(new ScriptBundle("~/bundles/clientBootstrapjs").Include(
                    "~/Scripts/client/popper.min.js",
                    "~/Scripts/client/bootstrap.min.js",
                    "~/Scripts/client/plugins.js",
                    "~/Scripts/client/active.js",
                    "~/Scripts/client/scripts.js"
                    ));

            //bundles.Add(new StyleBundle("~/Content/clientCss").Include(
            //          "~/Content/client/css/bootstrap.min.css",
            //          "~/Content/client/css/plugins.css",
            //          //"~/Content/client/css/style.css",
            //          "~/Content/client/css/style11.css",
            //          "~/Content/client/css/custom.css",
            //          "~/Content/client/Site.css"
            //          ));
            #endregion

            #region Admin Section

            bundles.Add(new StyleBundle("~/content/smartadmin")
                .Include(
                "~/content/admin/css/bootstrap.min.css",
                "~/content/admin/css/style.css",
                "~/scripts/admin/plugins/font-awesome/css/font-awesome.min.css",
                "~/scripts/admin/plugins/switchery/switchery.min.css",
                "~/scripts/admin/plugins/jvectormap/jquery-jvectormap.css",
                "~/scripts/admin/plugins/select2/select2.min.css",
                "~/scripts/admin/plugins/bootstrap-validator/bootstrapValidator.min.css",
                "~/scripts/admin/plugins/bootstrap-datepicker/bootstrap-datepicker.css",
                "~/scripts/admin/plugins/datatables/media/css/dataTables.bootstrap.css",
                "~/scripts/admin/plugins/datatables/extensions/Responsive/css/dataTables.responsive.css",
                "~/content/admin/css/demo/jquery-steps.min.css",
                "~/content/admin/css/demo/jasmine.css",
                "~/scripts/admin/plugins/pace/pace.min.css",
                "~/content/admin/css/your_style.min.css"
                ));

            bundles.Add(new ScriptBundle("~/scripts/smartadmin").Include(
                "~/scripts/admin/js/bootstrap.min.js",
                "~/scripts/admin/js/app.config.js",
                "~/scripts/admin/plugins/fastclick/fastclick.min.js",
                "~/scripts/admin/plugins/metismenu/metismenu.min.js",
                "~/scripts/admin/plugins/switchery/switchery.min.js",
                "~/scripts/admin/plugins/parsley/parsley.min.js",
                "~/scripts/admin/jquery-steps/jquery-steps.min.js",
                "~/scripts/admin/plugins/select2/select2.min.js",
                "~/scripts/admin/plugins/bootstrap-datepicker/bootstrap-datepicker.js",
                "~/scripts/admin/plugins/bootstrap-wizard/jquery.bootstrap.wizard.min.js",
                "~/scripts/admin/plugins/masked-input/bootstrap-inputmask.min.js",
                "~/scripts/admin/plugins/bootstrap-validator/bootstrapValidator.min.js",
                "~/scripts/admin/plugins/screenfull/screenfull.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/full-calendar").Include(
                "~/scripts/admin/plugins/moment/moment.min.js",
                "~/scripts/admin/plugins/fullcalendar/fullcalendar.min.js"
            ));


            bundles.Add(new ScriptBundle("~/scripts/datatables").Include(
                "~/scripts/admin/plugins/datatables/media/js/jquery.dataTables.js",
                "~/scripts/admin/plugins/datatables/media/js/dataTables.bootstrap.js",
                "~/scripts/admin/plugins/datatables/extensions/Responsive/js/dataTables.responsive.min.js"
            ));

            bundles.Add(new ScriptBundle("~/scripts/vector-map").Include(
                "~/scripts/admin/plugins/jvectormap/jquery-jvectormap.min.js",
                "~/scripts/admin/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"
            ));
            bundles.Add(new ScriptBundle("~/scripts/charts").IncludeDirectory("~/scripts/admin/plugins/flot-charts", "jquery.flot.*"));
            #endregion

            #region Layout Page
            bundles.Add(new StyleBundle("~/content/layout")
                .Include(
                //"~/Content/client/css/plugins.css",
                "~/Content/client/css/style11.css",
                "~/Content/client/css/jquery-ui.css",
                "~/Content/client/css/custom.css",
                "~/Content/client/css/plugins/slick.min.css",
                "~/Content/client/css/change-color.css",
                "~/Content/client/Site.css"
                ));

            bundles.Add(new StyleBundle("~/content/pluginscss")
                .Include(
                "~/Content/client/css/plugins/fakeloader.css",
                "~/Content/client/css/plugins/font-awesome.min.css",
                "~/Content/client/css/plugins/slick.min.css",
                "~/Content/client/css/plugins/meanmenu.css",
                "~/Content/client/css/plugins/lightgallery.min.css",
                "~/Content/client/css/plugins/nice-select.css",
                "~/Content/client/css/plugins/pe-icon-7-stroke.css",
                "~/Content/client/css/plugins/icofont.min.css/",
                "~/Content/client/css/plugins/odometer-theme-default.css",
                "~/Content/client/css/plugins/datepicker.min.css",
                "~/Content/client/custom/toastr.min.css"
                ));
            //bundles.Add(new ScriptBundle("~/content/toastr").Include(
            //    "~/Content/client/custom/toastr.min.css"
            //));

            bundles.Add(new ScriptBundle("~/scripts/layoutheader").Include(
               // "~/Scripts/client/vendor/jquery-3.2.1.min.js",
               "~/Scripts/client/vendor/jquery-1.10.2.js",
               "~/Scripts/client/vendor/jquery-ui.js",
                "~/Scripts/admin/js/moment.js",
                "~/Scripts/system/jquery.validate.js",
                "~/Scripts/system/jquery.validate.unobtrusive.js",
                "~/Scripts/custom/toastr.min.js"
            ));

            bundles.Add(new ScriptBundle("~/scripts/layoutfooter").Include(
                "~/Scripts/system/jquery.bootpag.min.js",
                "~/Scripts/client/vendor/modernizr-3.5.0.min.js",
                "~/Scripts/client/popper_1.14.7.min.js",
                "~/Scripts/client/plugins.js",
                "~/Scripts/client/scripts.js",
                "~/Scripts/client/bootstrap_4.3.1.min.js"
            ));
            #endregion

            BundleTable.EnableOptimizations = true;
        }
    }
}
