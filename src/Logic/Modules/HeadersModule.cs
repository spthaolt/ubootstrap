using System;
using System.Web;

namespace Bootstrap.Logic.Modules
{
    public class HeadersModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.EndRequest += ClearHeaders;
        }

        void ClearHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Remove("ETag");
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
        }

        public void Dispose()
        {
        }
    }
}
