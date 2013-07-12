using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;

namespace Website.Mvc.App_Start
{
    using System.Collections.Generic;
    using System.IO;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssTransformer = new CssTransformer();
            var jsTransformer = new JsTransformer();
            var nullOrderer = new NullOrderer();
            
            // Default theme CSS bundle - This bundle can be used if user does not want to bother with LESS
            // TODO: Check if files exist before adding to bundle!
            bundles.Add(new StyleBundle("~/bundles/theme.css").Include(
                "~/css/bootstrap/bootstrap.css",                   // Standard bootstrap css
                "~/css/font/font-awesome.css",                     // Standard font-awesome css
                "~/css/bootstrap/theme.css"                        // Custom CSS - add classes here to style your site
            ));

            // Responsive theme CSS bundle 
            bundles.Add(new StyleBundle("~/bundles/responsivetheme.css").Include(
                "~/css/bootstrap/bootstrap.css",                   // Standard bootstrap css
                "~/css/font/font-awesome.css",                     // Standard font-awesome css
                "~/css/bootstrap/theme.css",                       // Custom CSS - add classes here to style your site
                "~/css/bootstrap/bootstrap-responsive.css"         // Standard bootstrap responsive css
            ));

            // Right-to-left CSS bundle (used as an alternative to the above for Arabic, Hebrew etc)
            bundles.Add(new StyleBundle("~/bundles/theme.rtl").Include(
                "~/css/bootstrap/bootstrap.rtl.css",               // Standard bootstrap css
                "~/css/font/font-awesome.css",                     // Standard font-awesome css
                "~/css/bootstrap/theme.rtl.css"                    // Custom CSS - add classes here to style your site 
                //,"~/css/bootstrap/bootstrap-responsive.rtl.css"
            ));

            // Default CSS theme processed from Bootstrap LESS files
            var bootstrapLess = new Bundle("~/bundles/theme.less").Include(
                "~/less/bootstrap/bootstrap.less",                 // Standard bootstrap less
                "~/less/font/font-awesome.less",                   // Standard font-awesome less
                "~/less/bootstrap/theme.less"                      // Custom less file - add classes here to style your site
            );
            bootstrapLess.Transforms.Add(cssTransformer);
            bootstrapLess.Orderer = nullOrderer;
            bundles.Add(bootstrapLess);

            // Responsive CSS theme processed from Bootstrap LESS files
            var bootstrapLessResponsive = new Bundle("~/bundles/responsivetheme.less").Include(
                "~/less/bootstrap/bootstrap.less",                // Standard bootstrap less
                "~/less/font/font-awesome.less",                    // Standard font-awesome less
                "~/less/bootstrap/theme.less",                    // Custom less file - add classes here to style your site
                "~/less/bootstrap/responsive.less"                // Standard bootstrap responsive less
            );
            // Use the Bundle Transformer extension to the MS Optimisations framework to parse and minify the less files
            bootstrapLessResponsive.Transforms.Add(cssTransformer);
            bootstrapLessResponsive.Orderer = nullOrderer;
            bundles.Add(bootstrapLessResponsive); 

            // All site JS files (NB JQuery itself is served via CDN bundle below)
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/scripts/bootstrap/bootstrap-transition.js",          // Standard bootstrap JS - include in this order or there may be errors in the bundle
                "~/scripts/bootstrap/bootstrap-alert.js",
                "~/scripts/bootstrap/bootstrap-modal.js",
                // "~/scripts/bootstrap/bootstrap-dropdown.js",
                "~/scripts/bootstrap/bootstrap-scrollspy.js",
                "~/scripts/bootstrap/bootstrap-tab.js",
                "~/scripts/bootstrap/bootstrap-tooltip.js",
                "~/scripts/bootstrap/bootstrap-popover.js",
                "~/scripts/bootstrap/bootstrap-button.js",
                "~/scripts/bootstrap/bootstrap-collapse.js",
                "~/scripts/bootstrap/bootstrap-carousel.js",
                "~/scripts/bootstrap/bootstrap-typeahead.js",
                "~/scripts/site.js"                                     // Custom JS - add your custom JS to this file
            ));                              

            // Enable CDN support 
            bundles.UseCdn = true;   

            // CDN jQuery with local fallback
            var jqueryCdnPath = "//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.9.1.min.js";
            bundles.Add(new ScriptBundle("~/bundles/jquery",
                        jqueryCdnPath).Include(
                        "~/scripts/jquery-{version}.js"));

            // CDN jQuery validation
            var jqueryValidateCdnPath = "//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js";
            bundles.Add(new ScriptBundle("~/bundles/jqueryval",
                        jqueryValidateCdnPath).Include(
                        "~/scripts/jquery.validate.js"));

            // CDN MS Unobtrusive clientside validation script
            var jqueryValidateUnobtrusiveCdnPath = "//ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.min.js";
            bundles.Add(new ScriptBundle("~/bundles/jqueryvalunob",
                        jqueryValidateUnobtrusiveCdnPath).Include(
                        "~/scripts/jquery.validate.unobtrusive.js"));

            // HTML 5 shim for IE7
            var html5shimCdnPath = "//html5shim.googlecode.com/svn/trunk/html5.js";
            bundles.Add(new ScriptBundle("~/bundles/html5shim",
                        html5shimCdnPath).Include(
                        "~/scripts/html5.js"));

            // Force the bundles to minify, regardle of compilation debug attribute in web.config.
            // BundleTable.EnableOptimizations = true;


        }

        //TODO: FilesExist method to return a list of filename strings for Bundles
        private static IEnumerable<string> FilesExist(IEnumerable<string> files)
        {
            var retVal = new List<string>();
            foreach (var item in files)
            {
                if (File.Exists(item)) retVal.Add(item);
            }

            return retVal;
        }
    }
}