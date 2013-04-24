using System.Web.Configuration;
using System.Web.Routing;
using Combres;

using System.Web.Optimization;

namespace Bootstrap.Logic.Global
{
    public static class BootstrapPackage
    {
        public static void PreStart()
        {
            var webConfig = WebConfigurationManager.OpenWebConfiguration("~");
            var combres = webConfig.GetSection("combres");
            if (combres != null)
            {
                RouteTable.Routes.AddCombresRoute("Combres");

            }

            //BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}
