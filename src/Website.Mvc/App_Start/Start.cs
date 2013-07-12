using System.Web.Optimization;
using Umbraco.Core;

namespace Website.Mvc.App_Start
{
    public class MvcStartup : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }
    }
}