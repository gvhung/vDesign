using System.Web.Optimization;

namespace WebUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/yepnope").Include(
                "~/Scripts/yepnope.js"));            

            bundles.Add(new ScriptBundle("~/bundles/helpers").Include(
                "~/Scripts/view.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/jszip.min.js",
                "~/Scripts/jquery.mb.browser.min.js",
                "~/Scripts/modernizr.2.5.3.js",
                "~/Scripts/jquery.mousewheel.js",

                "~/Scripts/jquery.cookie-1.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.10.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/checkboxes").Include(
                    "~/Scripts/iphone-style-checkboxes.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo.all.min.js",
                "~/Scripts/kendo.aspnetmvc.min.js",
                "~/Scripts/kendo.culture.ru.js"));

            bundles.Add(new ScriptBundle("~/bundles/api")
                .Include("~/Scripts/common.js")
                .Include("~/Scripts/templates.js")
                .Include("~/Scripts/api.js")
                .Include("~/Scripts/application.js")
                .Include("~/Scripts/wraps.js")
                .Include("~/Scripts/bootstrap/bootstrap.js")
                .Include("~/Scripts/jquery.pba.js"));

            bundles.Add(new ScriptBundle("~/bundles/addons").Include(
                "~/Scripts/jsPlumb/jquery.jsPlumb-1.5.5-min.js",
                "~/Scripts/underscore-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/underscore").Include(
                "~/Scripts/underscore-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet").Include(
                "~/Scripts/leaflet.js",
                "~/Scripts/leaflet.extensions.js",
                "~/Scripts/wicket.js",
                "~/Scripts/wicket-leaflet.js",
                "~/Scripts/leaflet.draw.js",
                "~/Scripts/leaflet.draw.ru.js"));

            bundles.Add(new StyleBundle("~/Content/leaflet").Include(
                "~/Content/css/leaflet.css",
                "~/Content/css/leaflet.draw.css"));

            bundles.Add(new StyleBundle("~/Content/main_css")
                .Include("~/Content/css/icons.css")
                .Include("~/Content/css/fonts/glyphicons-regular.css")
                .Include("~/Content/css/fonts/glyphicons-halflings-regular.css")
                .Include("~/Content/css/fonts/glyphicons-filetypes-regular.css")
                .Include("~/Content/css/fonts/glyphicons-social-regular.css")
                .Include("~/Content/css/site.css")
                .Include("~/Content/css/data-style.css"));

            bundles.Add(new StyleBundle("~/Content/animate_css")
                .Include("~/Content/css/animate.css")
                .Include("~/Content/css/timeline.css")
                .Include("~/Content/css/workflow.css")
                );

            bundles.Add(new StyleBundle("~/Content/aside")
            .Include("~/Content/css/aside.css")
                );

            bundles.Add(new StyleBundle("~/Content/editor/css")
                .Include("~/Content/css/editor.css"));

            bundles.Add(new StyleBundle("~/Content/checkboxes")
                .Include("~/Content/css/checkboxes.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrapcss")
                .Include("~/Content/css/bootstrap/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/kendo/bootstrapcss")
                .Include("~/Content/css/kendo/bootstrap/kendo.common-bootstrap.min.css")
                .Include("~/Content/css/kendo/bootstrap/kendo.bootstrap.css")
                .Include("~/Content/css/kendo/bootstrap/kendo.dataviz.bootstrap.min.css")
                .Include("~/Content/css/kendo/bootstrap/kendo-overrides.css")
                );

            bundles.Add(new StyleBundle("~/Content/kendo/materialcss")
                .Include("~/Content/css/kendo/material/css/kendo.common-material.min.css")
                .Include("~/Content/css/kendo/material/css/kendo.material.min.css")
                .Include("~/Content/css/fira/fira.css")
                .Include("~/Content/css/kendo/material/css/kendo-overrides.css")
                );

            bundles.Add(new StyleBundle("~/Content/gantt").Include("~/Scripts/gant/dhtmlxgantt.css"));
            bundles.Add(new ScriptBundle("~/bundles/gantt").Include("~/Scripts/gant/dhtmlxgantt.js",
                "~/Scripts/gant/locale/locale_ru.js"));

            bundles.Add(new StyleBundle("~/Content/account")
                .Include("~/Content/css/fira/fira.css")
                .Include("~/Content/css/account.css"));

            /* PUBLIC BUNDLE */
            bundles.Add(new StyleBundle("~/bundles/public_site")
                .Include("~/Content/css/site.main.css")
                .Include("~/Content/css/site.header.css")
                .Include("~/Content/css/site.footer.css")
                .Include("~/Content/css/site.content.css")
                .Include("~/Content/css/site.mainwidget.css")
                .Include("~/Content/css/site.override.bootstrap.css"));

            /* PUBLIC BUNDLE */
            bundles.Add(new StyleBundle("~/bundles/public_vendor")
                .Include("~/Content/css/animate.css")
                .Include("~/Content/css/icons.css")
                .Include("~/Content/css/perfect-scrollbar.min.css")
                .Include("~/Content/css/dialogfx.css")
                .Include("~/Content/css/fonts/glyphicons-regular.css")
                .Include("~/Content/css/fonts/glyphicons-halflings-regular.css")
                .Include("~/Content/css/fonts/glyphicons-filetypes-regular.css")
                .Include("~/Content/css/fonts/glyphicons-social-regular.css"));
        }
    }
}